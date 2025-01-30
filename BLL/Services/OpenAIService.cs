using BLL.ExternalSystems.Fizikal;
using BLL.Interfaces;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _client;
        private readonly IFizikalHandler _fizikalHandler;
        private readonly List<ChatMessage> _chatHistory = new();
        public OpenAIService(string apiKey, IFizikalHandler fizikalHandler)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
            }

            _client = new ChatClient(model: "gpt-4o", apiKey: apiKey);
            _fizikalHandler = fizikalHandler;
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
            }

            try
            {

                _chatHistory.Add(ChatMessage.CreateUserMessage(prompt));
                ChatCompletion completion = await _client.CompleteChatAsync(_chatHistory); // Ensure async call

                if (completion?.Content == null || completion.Content.Count == 0)
                {
                    Console.WriteLine("[ERROR]: Received empty response from OpenAI.");
                    return "No response received.";
                }

                string response = completion.Content[0].Text;

                _chatHistory.Add(ChatMessage.CreateAssistantMessage(response));

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: OpenAI API call failed - {ex.Message}");
                return "Error processing your request.";
            }
        }
        public async Task SetChat()
        {
            string promptBegin = "אתה מדריך חדר כושר ורופא , אתה יודע להמליץ איזה פעולות מתאימות לכל בן אדם.\r\nאני הולך להביא לך את המידע הבא שאתה חייב להשתמש בו כדי לענות למי שהולך לשלוח לך את ההודעות הבאות.\r\n";
            string addId = "וגם אם מבקשים ממך את השם של הכיתה , תשלח אותה עם הid ";
            string addJsonIntheEndOfMessage = "לכל שיעור שאתה מציג לי תשלח גם איך הוא מוצג כjson עם השדות הבאים :\r\nName , id , date \r\n";
            var fizikalclass = await _fizikalHandler.GetClassScheduleAsync();
            var fizikalSchedual = await _fizikalHandler.GetClassCategories();

            var prompt = promptBegin + "\n" + addId + "\n"+ addJsonIntheEndOfMessage + "\n" + fizikalclass.ToString() + "\n" + fizikalSchedual.ToString();
            ChatCompletion completion = await _client.CompleteChatAsync(prompt);
            _chatHistory.Clear();
            _chatHistory.Add(ChatMessage.CreateSystemMessage(prompt));
        }
    }
}
