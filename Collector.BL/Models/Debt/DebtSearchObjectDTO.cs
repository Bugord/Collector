using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Debt
{
    public class DebtSearchObjectDTO
    {
        public bool? IsClosed { get; set; }
        public bool? Overdued { get; set; }
        public bool? ReqUserOwe { get; set; }
        public bool? IsSynchronized { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? FriendId { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public float? ValueMoreThan { get; set; }
        public float? ValueLessThan { get; set; }
        public int Offset { get; set; }
        public int Take { get; set; }
    }
}
