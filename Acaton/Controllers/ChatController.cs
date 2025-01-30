
using BLL.ExternalSystems.Fizikal;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AcatonApi.Controllers
{
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IFizikalHandler _fizikalHandler;
        public ChatController(IChatService chatService, IFizikalHandler fizikalHandler)
        {
            _chatService = chatService;
            _fizikalHandler = fizikalHandler;
        }

        [HttpGet("messages/{room}")]
        public IActionResult GetMessages(string room)
        {
            return Ok(_chatService.GetConversation(room));
        }

        [HttpPost("send")] // Send a message via HTTP (not real-time)
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            await _chatService.SendMessageToRoom(message.Room, message.User, message.Text);
            return Ok();
        }

        [HttpGet("get-fizikla")]
        public async Task<IActionResult> GetFizikalClass()
        {
            var res = await _fizikalHandler.GetClassScheduleAsync();
            return Ok();
        }
    }
}
