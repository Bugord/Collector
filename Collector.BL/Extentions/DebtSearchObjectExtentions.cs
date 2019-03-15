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
            var currentDate = DateTime.UtcNow.Date;

            return debt => (!model.DebtOwner.HasValue || (debt.Owner.Id == reqUserId) == model.DebtOwner.Value) && //Is owner of debt
                           (!model.IsClosed.HasValue || debt.IsClosed == model.IsClosed) && //Is closed
                           (!model.IsMoney.HasValue || debt.IsMoney == model.IsMoney) && //Is closed
                           (!model.IsSynchronized.HasValue || debt.Synchronize == model.IsSynchronized) && //Is synchronized
                           (string.IsNullOrWhiteSpace(model.FriendName) || debt.Friend.Name.ToUpper().Equals(model.FriendName.ToUpper())) && //Friend name
                           (string.IsNullOrWhiteSpace(model.Name) || debt.Name.ToUpper().Contains(model.Name.ToUpper())) && //Name
                           (string.IsNullOrWhiteSpace(model.Description) || debt.Description.ToUpper().Contains(model.Description.ToUpper())) && //Description
                           (!model.Overdued.HasValue || (model.Overdued.Value //Overdued
                                ? debt.DateOfOverdue.Value.Date < currentDate
                                : debt.DateOfOverdue > currentDate)) &&
                           (!model.ReqUserOwe.HasValue || (model.ReqUserOwe.Value //Is debter
                                ? debt.IsOwnerDebter == (debt.Owner.Id == reqUserId)
                                : debt.IsOwnerDebter != (debt.Owner.Id == reqUserId))) &&
                           (!model.CreatedFrom.HasValue || debt.Created >= model.CreatedFrom) && //Created from
                           (!model.CreatedBefore.HasValue || debt.Created <= model.CreatedBefore) && //Created before
                           (!model.ValueLessThan.HasValue || debt.Value <= model.ValueLessThan) && //Value less than
                           (!model.ValueMoreThan.HasValue || debt.Value >= model.ValueMoreThan) //Value more than
                ;
        }
    }
}