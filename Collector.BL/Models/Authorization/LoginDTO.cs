using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Authorization
{
    public class LoginDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Email must be between 3 an 100")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Password must be between 3 an 100")]
        public string Password { get; set; }
    }
}
