using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Collector.DAO.Entities
{
    public class BaseEntity
    {
        public long ID { get; set; }
        public long CreatedBy { get; set; }
        public DateTime Created { get; set; }
        [DefaultValue(null)]
        public long? ModifiedBy { get; set; }
        [DefaultValue(null)]
        public DateTime? Modified { get; set; }
    }
}
