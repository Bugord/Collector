using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Collector.DAO.Entities
{
    public class Debt : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public long OwnerId { get; set; }
        [Required]
        public long FriendId { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool Synchronize { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public float Value { get; set; }
        public bool IsOwnerDebter { get; set; }
    }
}
