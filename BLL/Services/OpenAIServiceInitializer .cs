using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using Microsoft.Extensions.Hosting;

namespace BLL.Services
{
    public class OpenAIServiceInitializer : IHostedService
    {
        private readonly IOpenAIService _openAIService;

        public OpenAIServiceInitializer(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _openAIService.SetChat();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
