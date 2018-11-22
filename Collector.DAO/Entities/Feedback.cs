using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Collector.DAO.Entities
{
    public class Feedback : BaseEntity
    {
    //    [ForeignKey("Creator")]
    //    public long CreatorId { get; set; }
    //    [ForeignKey("ClosedBy")]
    //    public long ClosedById { get; set; }
        public virtual User Creator { get; set; }
        public ICollection<FeedbackMessage> Messages { get; set; }
        public virtual User ClosedBy { get; set; }
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool IsClosed { get; set; }
        public DateTime? Closed { get; set; }
    }
}
