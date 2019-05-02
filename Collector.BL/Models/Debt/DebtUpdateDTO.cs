using System;
using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Debt
{
    public class DebtUpdateDTO
    {
        [Required]
        public long DebtId { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Debt name must be between 3 an 100")]
        public string Name { get; set; }
        [Required]
        public long FriendId { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public bool Synchronize { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be not negative and less than 2 147 483 647")]

        public decimal? Value { get; set; }
        public long? CurrencyId { get; set; }
        public decimal? CurrentValue { get; set; }
        public bool IsOwnerDebter { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? DateOfOverdue { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMoney { get; set; }
    }
}
