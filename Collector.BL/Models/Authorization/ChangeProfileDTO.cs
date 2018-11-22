using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Collector.BL.Models.Authorization
{
    public class ChangeProfileDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 an 100")]
        public string Username { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Email must be between 3 an 100")]
        public string Email { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "FirstName must be between 3 an 100")]
        public string FirstName { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "MinimumLength must be between 3 an 100")]
        public string LastName { get; set; }
        public IFormFile AvatarFile { get; set; }
    }
}
