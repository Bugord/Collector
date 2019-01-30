using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{
    public class Change : BaseEntity
    {
        public User ChangedBy { get; set; }
        [Required]
        public Debt ChangedDebt { get; set; }
        [Required]
        public ICollection<FieldChange> FieldChanges { get; set; }

    }
}
