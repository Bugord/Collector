using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Chat
{
    public class ChatReturnDTO
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public string SentTo { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime Created { get; set; }
    }
}
