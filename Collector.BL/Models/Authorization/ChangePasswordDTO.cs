using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Authorization
{
    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Old password must be between 3 an 100")]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "New password must be between 3 an 100")]
        public string NewPassword { get; set; }
    }
}
