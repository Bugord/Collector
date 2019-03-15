using System;
using System.Collections.Generic;
using Collector.BL.Models.Authorization;
using Collector.BL.Models.FriendList;

namespace Collector.BL.Models.Debt
{
    public class DebtReturnDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long FriendId { get; set; }
        public bool Synchronize { get; set; }
        public string Description { get; set; }
        public float Value { get; set; } //todo change to float?
        public long CurrencyId { get; set; }
        public float CurrentValue { get; set; } //todo change to float?
        public bool IsOwner { get; set; }
        public DateTime Created { get; set; }
        public bool IsOwnerDebter { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? DateOfOverdue { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMoney { get; set; }
    }
}
