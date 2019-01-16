using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Chat
{
    public class ChatGetMessagesDTO
    {
        public string ChatWithUsername { get; set; }
        public int SkipMessages { get; set; }
        public int TakeMessages{ get; set; }
    }
}
