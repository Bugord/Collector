using System;

namespace Collector.BL.Models.Debt
{
    public class PaymentDTO
    {
        public DateTime Date;
        public float? Value;
        public string DebtName;
        public CurrencyReturnDTO Currency;
        public bool IsOwnerPay;
        public string AvatarUrl;
        public string Username;
    }
}