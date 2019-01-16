using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class DebtSearchObjectExtentions
    {
        public static Expression<Func<Debt, bool>> GetExpression(this DebtSearchObjectDTO model, long reqUserId)
        {
            return debt => (!model.IsClosed.HasValue || debt.IsClosed == model.IsClosed) && //Is closed
                           (!model.IsSynchronized.HasValue || debt.Synchronize == model.IsSynchronized) && //Is synchronized
                           (string.IsNullOrWhiteSpace(model.FriendName) || debt.Friend.Name.Equals(model.FriendName,
                                StringComparison.CurrentCultureIgnoreCase)) && //Friend name
                           (string.IsNullOrWhiteSpace(model.Name) || debt.Name.Contains(model.Name,
                                StringComparison.CurrentCultureIgnoreCase)) && //Name
                           (string.IsNullOrWhiteSpace(model.Description) || debt.Description.Contains(model.Description,
                                StringComparison.CurrentCultureIgnoreCase)) && //Description
                           (!model.Overdued.HasValue || (model.Overdued.Value //Overdued
                                ? debt.DateOfOverdue < DateTime.UtcNow
                                : debt.DateOfOverdue > DateTime.UtcNow)) &&
                           (!model.ReqUserOwe.HasValue || (model.ReqUserOwe.Value //Is debter
                                ? debt.IsOwnerDebter == (debt.Owner.Id == reqUserId)
                                : debt.IsOwnerDebter != (debt.Owner.Id == reqUserId))) &&
                           (!model.CreatedFrom.HasValue || debt.Created >= model.CreatedFrom) && //Created from
                           (!model.CreatedBefore.HasValue || debt.Created <= model.CreatedBefore) &&//Created before
                           (!model.ValueLessThan.HasValue || debt.Value <= model.ValueLessThan) && //Value less than
                           (!model.ValueMoreThan.HasValue || debt.Value >= model.ValueMoreThan) //Value more than
                ;
        }
    }
}