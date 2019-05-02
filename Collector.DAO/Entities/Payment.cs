using System;
using System.ComponentModel.DataAnnotations;
using static System.Double;

namespace Collector.DAO.Entities
{
    public class Payment : BaseEntity
    {
        [Required] public User Payer { get; set; }
        [Required] public User UserToPay { get; set; }
        [Required] public Debt Debt { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        public decimal? Value { get; set; }
        public Currency Currency { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        [Required] public PaymentType PaymentType { get; set; }
        [Required] public Status Status { get; set; }
    }

    public enum PaymentType
    {
        Stripe,
        Notification
    }

    public enum Status
    {
        Pending,
        Accepted,
        Denied
    }
}