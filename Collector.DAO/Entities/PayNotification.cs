using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{
    public class PayNotification : BaseEntity
    {
        public User Payer { get; set; }
        public User UserToPay { get; set; }
        [Required]
        public Debt Debt { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        public decimal? Value { get; set; }
        public string Message { get; set; }
        public bool Confirmed { get; set; } 
        public bool Approved { get; set; } 
    }
}
