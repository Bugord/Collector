using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.BL.Models.Feedback
{
    public class FeedbackMessageAddDTO
    {
        [Required]
        public long FeedbackId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
    }
}
