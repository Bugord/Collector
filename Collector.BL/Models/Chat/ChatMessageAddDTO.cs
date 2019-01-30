using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.BL.Models.Chat
{
    public class ChatMessageAddDTO
    {
        public string SendToUsername { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Chat message text length must be between 1 and 500")]
        public string MessageText { get; set; }
    }
}
