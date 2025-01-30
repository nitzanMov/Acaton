using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ChatMessage
    {
        public string Room { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
