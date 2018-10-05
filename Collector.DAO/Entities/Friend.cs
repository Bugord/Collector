using System.ComponentModel.DataAnnotations;
namespace Collector.DAO.Entities
{
    public class Friend : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string OwnersName { get; set; }
        [Required]
        public long OwnerId { get; set; }
        [Required]
        public bool IsSynchronized { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string UsersName { get; set; }
        public long? UserId { get; set; }
        public long? InviteId { get; set; }
    }
}
