using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class PaymentExtentions
    {
        public static PayNotificationReturnDTO ToNotificationReturnDTO(this Payment payment)
        {
            return new PayNotificationReturnDTO
            {
                Currency = payment.Currency.CurrencySymbol,
                Message = payment.Message,
                DebtDescription = payment.Debt.Description,
                DebtName = payment.Debt.Name,
                Value = payment.Value,
                isMoney = payment.Debt.IsMoney,
                PayerUsername = payment.Payer.Username,
                Id = payment.Id,
                RowVersion = payment.RowVersion
            };
        }

        public static PaymentDTO ToPaymentDTO(this Payment payment, long ownerId)
        {
            return new PaymentDTO
            {
                CurrencyId = payment.Debt.Currency?.Id,
                Value = payment.Value,
                Date = payment.Created.Date,
                DebtName = payment.Debt.Name,
                IsOwnerPay = payment.Payer.Id == ownerId,
                AvatarUrl = payment.Payer.Id == ownerId
                    ? payment.UserToPay.AratarUrl
                    : payment.Payer.AratarUrl,
                Username = payment.Payer.Id == ownerId
                    ? payment.UserToPay.Username
                    : payment.Payer.Username,
            };
        }
    }
}