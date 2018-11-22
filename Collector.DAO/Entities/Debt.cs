using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Collector.DAO.Entities
{
    public class Debt : BaseEntity
    {
        [Required]
        public User Owner { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public float Value { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public Friend Friend { get; set; }
        [Required]
        public bool Synchronize { get; set; }
        public bool IsOwnerDebter { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? DateOfOverdue { get; set; }
        public ICollection<Change> Changes { get; set; }
    }
}