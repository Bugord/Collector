using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.BL.Models.Feedback
{
    public class FeedbackAddDTO
    {
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

    }
}
