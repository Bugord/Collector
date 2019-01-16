using System.ComponentModel.DataAnnotations;
namespace Collector.DAO.Entities
{
    public class Friend : BaseEntity
    {
        public long OwnerId { get; set; }
        public virtual User Owner { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public bool IsSynchronized { get; set; }
        public virtual User FriendUser { get; set; }
        public long? InviteId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
