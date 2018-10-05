using System.ComponentModel.DataAnnotations;
namespace Collector.DAO.Entities
{
    public class Invite : BaseEntity
    {
        [Required]
        public long OwnerId { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public long FriendId { get; set; }
        [Required]
        public bool Approved { get; set; }
    }
}
