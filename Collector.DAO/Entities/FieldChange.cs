using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.DAO.Entities
{
    public class FieldChange : BaseEntity
    {
        public Change Change { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
