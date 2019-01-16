using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required]
        public bool Confirmed { get; set; }

        public string AratarUrl { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
    }

    public enum Role
    {
        Admin = 1,
        Moderator,
        User
    }
}
