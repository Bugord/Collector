using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.DAO.Entities
{
    public class Change : BaseEntity
    {
        public User ChangedBy { get; set; }
        public Debt ChangedDebt { get; set; }
        public ICollection<FieldChange> FieldChanges { get; set; }

    }
}
