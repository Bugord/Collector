using System;
using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Debt
{
    public class DebtAddDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Debt name must be between 3 an 100")]
        public string Name { get; set; }
        [Required]
        public long FriendId { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public bool Synchronize { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Value must be not negative and less than 2 147 483 647")]
        public float? Value { get; set; }
        public long? CurrencyId { get; set; }
        public float? CurrentValue { get; set; }
        public bool IsOwnerDebter { get; set; }
        public DateTime? DateOfOverdue { get; set; }
        [Required]
        public bool IsMoney { get; set; }
    }
}
