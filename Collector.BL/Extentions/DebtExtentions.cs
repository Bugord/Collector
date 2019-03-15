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
                Value = debt.Value ?? 0, //todo change to debt.value
                CurrentValue = debt.CurrentValue ?? 0,//todo change to debt.CurrentValue
                Name = debt.Name,
                IsOwner = isOwner,
                Created = debt.Created.Date,
                IsOwnerDebter = debt.IsOwnerDebter,
                RowVersion = debt.RowVersion,
                DateOfOverdue = debt.DateOfOverdue?.Date,
                IsClosed = debt.IsClosed,
                IsMoney = debt.IsMoney,
                CurrencyId = debt.Currency?.Id ?? 0,
            };
        }

        public static Debt DebtAddDTOToDebt(this DebtAddDTO model, Friend friend, User owner, Currency currency)
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
                DateOfOverdue = model.DateOfOverdue,
                IsMoney = model.IsMoney,
                CurrentValue = model.CurrentValue,
                Currency = currency
            };
        }

        public static Debt Update(this Debt debt, DebtUpdateDTO model, Friend friend, Currency currency,
            out List<FieldChange> fieldChanges)
        {
            fieldChanges = new List<FieldChange>();

            //debt.Name = debt.Name.HandleChange(model.Name, nameof(debt.Name), out var fieldChange);
            //if (fieldChange != null)
            //    fieldChanges.Add(fieldChange);

            debt.Name = model.Name;
            
            debt.Description = debt.Description.HandleChange(model.Description, nameof(debt.Description), out var fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);
            
            debt.Friend = debt.Friend.HandleChange(friend, nameof(debt.Friend), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.Synchronize = debt.Synchronize.HandleChange(model.Synchronize, nameof(debt.Synchronize), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            //debt.Value = debt.Value.HandleChange(model.Value, nameof(debt.Value), out fieldChange);
            //if (fieldChange != null)
            //    fieldChanges.Add(fieldChange);

            debt.Value = model.Value;
            debt.CurrentValue = model.CurrentValue;
            debt.Currency = currency;

            debt.IsOwnerDebter = debt.IsOwnerDebter.HandleChange(model.IsOwnerDebter, nameof(debt.IsOwnerDebter), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.DateOfOverdue = debt.DateOfOverdue.HandleChange(model.DateOfOverdue, nameof(debt.DateOfOverdue), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.IsClosed = debt.IsClosed.HandleChange(model.IsClosed, nameof(debt.IsClosed), out fieldChange);
            if (fieldChange != null)
                fieldChanges.Add(fieldChange);

            debt.IsMoney = debt.IsClosed.HandleChange(model.IsMoney, nameof(debt.IsMoney), out fieldChange);
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