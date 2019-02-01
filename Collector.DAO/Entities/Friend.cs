using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Collector.DAO.Entities
{
    public class Friend : BaseEntity
    {
        [Required]
        public long OwnerId { get; set; }
        [Required]
        public virtual User Owner { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public bool IsSynchronized { get; set; }
        public virtual User FriendUser { get; set; }
        public long? InviteId { get; set; }
        public ICollection<Debt> Debts { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
