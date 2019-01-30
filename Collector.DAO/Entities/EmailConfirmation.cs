using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Collector.DAO.Entities
{
    public class EmailConfirmation : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public string VerificationToken { get; set; }
        public DateTime? ConfirmationTime { get; set; }
        [Required]
        public bool Used { get; set; }

    }
}
