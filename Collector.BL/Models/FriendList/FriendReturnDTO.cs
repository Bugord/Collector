using Collector.BL.Models.Authorization;

namespace Collector.BL.Models.FriendList
{
    public class FriendReturnDTO
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public bool IsSynchronized { get; set; }
        public UserReturnDTO FriendUser { get; set; }
        public decimal Owe1 { get; set; }
        public decimal Owe2 { get; set; }
    }
}
