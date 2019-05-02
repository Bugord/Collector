using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Authorization;

namespace Collector.BL.Models.Debt
{
    public class PayNotificationReturnDTO
    {
        public long Id;
        public string PayerUsername;
        public string DebtDescription;
        public bool isMoney;
        public decimal? Value;
        public string Currency;
        public string DebtName;
        public string Message;
        public byte[] RowVersion;
    }
}
