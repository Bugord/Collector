using System.ComponentModel.DataAnnotations;

namespace Collector.BL.Models.FriendList
{
    public class FriendUpdateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be between {1} and {2}")]
        public string Name { get; set; }

        [Required] public long Id { get; set; }
    }
}