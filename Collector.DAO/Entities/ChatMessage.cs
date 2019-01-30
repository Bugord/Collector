using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{

    public class ChatMessage : BaseEntity
    {
        [Required]
        public User Author { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
        public User SentTo { get; set; }
        [Required]
        public ChatMessageType Type { get; set; }
    }

    public enum ChatMessageType
    {
        Text,
        Url,
        Image,
        YoutubeVideo,
        Coub,
        Audio
    }
}
