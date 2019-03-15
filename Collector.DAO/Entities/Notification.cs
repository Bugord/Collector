using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        public User Recipient { get; set; }
        [MaxLength(256)]
        public string Message { get; set; }
        [Required]
        public bool Confirmed { get; set; }
    }
}
