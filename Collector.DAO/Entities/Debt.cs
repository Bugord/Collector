using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Collector.DAO.Entities
{
    public class Debt : BaseEntity
    {
        [Required]
        public User Owner { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "Money")]
        public decimal? Value { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "Money")]
        public decimal? PendingValue { get; set; }
        [Range(0, int.MaxValue)]
        public decimal? CurrentValue { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public Friend Friend { get; set; }
        [Required]
        public bool Synchronize { get; set; }
        [Required]
        public bool IsOwnerDebter { get; set; }
        [Required]
        public bool IsClosed { get; set; }
        public DateTime? DateOfOverdue { get; set; }
        public bool IsMoney { get; set; }
        public Currency Currency { get; set; }
        public ICollection<Change> Changes { get; set; }
        public ICollection<PayNotification> PayNotifications { get; set; }
    }
}