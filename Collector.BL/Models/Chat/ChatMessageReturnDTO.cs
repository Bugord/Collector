using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Chat
{
    public class ChatMessageReturnDTO
    {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string Text { get; set; }
        public string SentTo { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime Created { get; set; }
        public long Id { get; set; }
        public string Type { get; set; }
    }
}
