using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Debt
{
    public class ChangeSearchDTO
    {
        public long Id { get; set; }
        public int Offset { get; set; }
        public int Take { get; set; }
    }
}
