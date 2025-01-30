using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BLL.Hubs
{
    public class ChatHub : Hub
    {
        public static readonly ConcurrentDictionary<string, List<string>> Messages =  new ConcurrentDictionary<string, List<string>>();
        private readonly IOpenAIService _openAIService;

        public ChatHub(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task Message(string jsonMessage)
        {
            try
            {
                // Deserialize ההודעה לאובייקט
                var messageObj = JsonSerializer.Deserialize<ChatMessage>(jsonMessage);

                if (messageObj != null)
                {
                    // שליחת האובייקט לכל הלקוחות
                    await Clients.All.SendAsync("ReceiveMessage", messageObj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בפענוח JSON: {ex.Message}");
            }
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"📢 משתמש הצטרף לחדר {roomName}!");
        }
        public async Task SendMessageToRoom(string message)
        {
            var chatMessage = await _openAIService.GetCompletionAsync(message);
            await Clients.Group("2").SendAsync("SendMessageToRoom", chatMessage);
        }
    }
}

