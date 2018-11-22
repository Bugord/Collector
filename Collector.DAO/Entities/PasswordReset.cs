using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Collector.DAO.Entities
{
    public class PasswordReset : BaseEntity
    {
        [Required]
        public User User { get; set; }
        [Required]
        public string VerificationToken { get; set; }
        public DateTime ExpirationTime { get; set; }
        [Required]
        public bool Used { get; set; }
        public DateTime? ResetDate { get; set; }

    }
}
