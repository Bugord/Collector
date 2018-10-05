using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.FriendList
{
    public class FriendAcceptDTO
    {
        [Required]
        public long InviteId { get; set; }
        [Required]
        public bool Accepted { get; set; }
        [StringLength(16, MinimumLength = 3, ErrorMessage = "OwnersName must be between 3 an 16")]
        public string UsersName { get; set; }
        public long? FriendId { get; set; }
    }
}
