using BLL.ExternalSystems.Fizikal;
using BLL.Helpers;
using BLL.Hubs;
using BLL.Interfaces;
using BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddSignalR(option =>
{
    option.KeepAliveInterval = TimeSpan.FromSeconds(10);
    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<ChatHub>();
builder.Services.AddSingleton<IChatService,ChatService>();
builder.Services.AddSingleton<IFizikalHandler, FizikalHandler>();
builder.Services.AddSingleton<IOpenAIService, OpenAIService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["OpenAI:ApiKey"];
    var fizikalHandler = sp.GetRequiredService<IFizikalHandler>();
    return new OpenAIService(apiKey, fizikalHandler);
});
builder.Services.AddHostedService<OpenAIServiceInitializer>();
builder.Services.AddHttpClient<HttpHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
