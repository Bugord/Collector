using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.FriendList
{
    public class FriendInviteDTO
    {
        [Required]
        public long FriendId { get; set; }
        [Required]
        public string FriendEmail { get; set; }
    }
}
