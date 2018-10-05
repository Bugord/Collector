using System;

namespace Collector.BL.Models.Debt
{
    public class DebtReturnDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long FriendId { get; set; }
        public bool Synchronize { get; set; }
        public string Description { get; set; }
        public float Value { get; set; }
        public bool IsOwner { get; set; }
        public DateTime Created { get; set; }
        public bool IsOwnerDebter { get; set; }
    }
}
