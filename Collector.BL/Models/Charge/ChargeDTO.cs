using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Charge
{
    public class ChargeDTO
    {
        public long Value { get; set; }
        public string Token { get; set; }
        public string Currency { get; set; }
    }
}
