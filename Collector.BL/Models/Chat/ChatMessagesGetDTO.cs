
namespace Collector.BL.Models.Chat
{
    public class ChatMessagesGetDTO
    {
        public string ChatWithUsername { get; set; }
        public int SkipMessages { get; set; }
        public int TakeMessages{ get; set; }
    }
}
