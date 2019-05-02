using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Debt
{
    public class DebtSearchObjectDTO
    {
        public bool? DebtOwner { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsMoney { get; set; }
        public bool? Overdued { get; set; }
        public bool? ReqUserOwe { get; set; }
        public bool? IsSynchronized { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FriendName { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public decimal? ValueMoreThan { get; set; }
        public decimal? ValueLessThan { get; set; }
        public int Offset { get; set; }
        public int Take { get; set; }
    }
}
