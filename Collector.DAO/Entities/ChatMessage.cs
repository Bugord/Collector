using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{

    public class ChatMessage : BaseEntity
    {
        public User Author { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
        public string SentTo { get; set; }
    }
}
