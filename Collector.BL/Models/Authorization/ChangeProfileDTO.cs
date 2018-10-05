using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.Authorization
{
    public class ChangeProfileDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 an 100")]
        public string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Email must be between 3 an 100")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "FirstName must be between 3 an 100")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "MinimumLength must be between 3 an 100")]
        public string LastName { get; set; }
    }
}
