using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class DebtExtentions
    {
        public static DebtReturnDTO DebtToReturnDebtDTO(this Debt debt, bool isOwner = true)
        {
            return new DebtReturnDTO
            {
                Id = debt.Id,
                FriendId = debt.FriendId,
                Synchronize = debt.Synchronize,
                Description = debt.Description,
                Value = debt.Value,
                Name = debt.Name,
                IsOwner = isOwner,
                Created = debt.Created,
                IsOwnerDebter = debt.IsOwnerDebter
            };
        }

        public static Debt DebtAddDTOToDebt(this DebtAddDTO model, long ownerId)
        {
            return new Debt
            {
                Name = model.Name,
                Description = model.Description,
                FriendId = model.FriendId,
                OwnerId = ownerId,
                CreatedBy = ownerId,
                Synchronize = model.Synchronize,
                Value = model.Value,
                IsOwnerDebter = model.IsOwnerDebter
            };
        }

        public static Debt Update(this Debt debt, DebtUpdateDTO model)
        {
            debt.Name = model.Name;
            debt.Description = model.Description;
            debt.FriendId = model.FriendId;
            debt.Synchronize = model.Synchronize;
            debt.Value = model.Value;
            debt.IsOwnerDebter = model.IsOwnerDebter;
            return debt;
        }
    }
}
