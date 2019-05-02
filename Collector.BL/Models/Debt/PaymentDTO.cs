using System;

namespace Collector.BL.Models.Debt
{
    public class PaymentDTO
    {
        public DateTime Date;
        public decimal? Value;
        public string DebtName;
        public long? CurrencyId;
        public bool IsOwnerPay;
        public string AvatarUrl;
        public string Username;
    }
}