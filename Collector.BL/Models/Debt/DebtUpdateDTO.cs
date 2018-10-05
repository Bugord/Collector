using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Debt
{
    public class DebtUpdateDTO
    {
        [Required]
        public long DebtId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Debt name must be between 3 an 100")]
        public string Name { get; set; }
        [Required]
        public long FriendId { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public bool Synchronize { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Value must be not negative and less than 2 147 483 647")]

        public float Value { get; set; }
        public bool IsOwnerDebter { get; set; }

    }
}
