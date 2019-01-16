using System;
using System.Collections.Generic;
using System.Linq;
using Collector.BL.Models.Debt;
using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class DebtExtentions
    {
        public static DebtReturnDTO DebtToReturnDebtDTO(this Debt debt, bool isOwner = true, Friend otherFriend = null)
        {
            return new DebtReturnDTO
            {
                Id = debt.Id,
                FriendId = isOwner ? debt.Friend.Id : otherFriend.Id,
                Synchronize = debt.Synchronize,
                Description = debt.Description,
                Value = debt.Value,
                Name = debt.Name,
                IsOwner = isOwner,
                Created = debt.Created,
                IsOwnerDebter = debt.IsOwnerDebter,
                RowVersion = debt.RowVersion,
                DateOfOverdue = debt.DateOfOverdue,
                IsClosed = debt.IsClosed,
            };
        }

        public static Debt DebtAddDTOToDebt(this DebtAddDTO model, Friend friend, User owner)
        {
            return new Debt
            {
                Name = model.Name,
                Description = model.Description,
                Friend = friend,
                Owner = owner,
                CreatedBy = owner.Id,
                Synchronize = model.Synchronize,
                Value = model.Value,
                IsOwnerDebter = model.IsOwnerDebter,
                DateOfOverdue = model.DateOfOverdue
            };
        }

        public static Debt Update(this Debt debt, DebtUpdateDTO model, Friend friend,
            out List<FieldChange> fieldChanges)
        {
            fieldChanges = new List<FieldChange>();

            debt.Name = debt.Name.HandleChange(model.Name, nameof(debt.Name), out var fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);
            
            debt.Description = debt.Description.HandleChange(model.Description, nameof(debt.Description), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);
            
            debt.Friend = debt.Friend.HandleChange(friend, nameof(debt.Friend), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.Synchronize = debt.Synchronize.HandleChange(model.Synchronize, nameof(debt.Synchronize), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.Value = debt.Value.HandleChange(model.Value, nameof(debt.Value), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.IsOwnerDebter = debt.IsOwnerDebter.HandleChange(model.IsOwnerDebter, nameof(debt.IsOwnerDebter), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.DateOfOverdue = debt.DateOfOverdue.HandleChange(model.DateOfOverdue, nameof(debt.DateOfOverdue), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.IsClosed = debt.IsClosed.HandleChange(model.IsClosed, nameof(debt.IsClosed), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.RowVersion = model.RowVersion;

            //debt.RowVersion.HandleChange(model.RowVersion, out fieldChange);
            //if (fieldChange != null)
            //    fieldChanges.Add(fieldChange);

            return debt;
        }
    }
}