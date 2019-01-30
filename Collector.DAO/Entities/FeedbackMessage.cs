using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Collector.DAO.Entities
{
    public class FeedbackMessage : BaseEntity
    {
        [Required]
        public Feedback Feedback { get; set; }
        [Required]
        public User Author { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
    }
}
