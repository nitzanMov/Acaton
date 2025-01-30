
using BLL.Hubs;
using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IOpenAIService _openAIService;
        public ChatService(IHubContext<ChatHub> hubContext, IOpenAIService openAIService)
        {
            _hubContext = hubContext;
            _openAIService = openAIService;
        }

        public async Task SendMessageToRoom(string room, string user, string message)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be null or empty.", nameof(room));
            var response = await _openAIService.GetCompletionAsync(message);

            if (!ChatHub.Messages.TryGetValue(room, out var messages))
            {
                messages = new List<string>();
                ChatHub.Messages[room] = messages; // Store the new list in the dictionary
            }

            // Add the user message and the OpenAI response to the room's message list
            messages.Add($"{user}: {message}");
            messages.Add($"OpenAI: {response}");
            // Add your custom logic here, if needed
            await _hubContext.Clients.Group(room).SendAsync("ReceiveMessage", user, message);
            await _hubContext.Clients.Group(room).SendAsync("ReceiveMessage", "OpenAI", response);
            

        }
        public List<string> GetConversation(string room)
        {
             var res =  ChatHub.Messages.TryGetValue(room, out var messages) ? messages : new List<string>();
            return res;
        }
    }
}
