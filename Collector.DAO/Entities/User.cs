using System.ComponentModel.DataAnnotations;

namespace Collector.DAO.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 3)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string LastName { get; set; }
    }

    public enum Role
    {
        Admin,
        User
    }
}
