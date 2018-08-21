using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.BL.Entity
{
    public class RegisterDto
    {
        [Required]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Username must be between 3 an 16")]
        public string Username { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Email must be between 3 an 25")]
        public string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Password must be between 3 an 20")]
        public string Password { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "FirstName must be between 3 an 16")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "MinimumLength must be between 3 an 16")]
        public string LastName { get; set; }
    }
}
