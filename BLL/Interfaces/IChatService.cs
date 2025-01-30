using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IChatService
    {
        Task SendMessageToRoom(string room, string user, string message);
        List<string> GetConversation(string room);
    }
}
