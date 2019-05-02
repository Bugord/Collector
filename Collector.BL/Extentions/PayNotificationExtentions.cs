using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class PayNotificationExtentions
    {
        public static PayNotificationReturnDTO ToPayNotificationReturnDTO(this PayNotification notification)
        {
            return new PayNotificationReturnDTO
            {
                Id = notification.Id,
                PayerUsername = notification.Payer.Username,
                DebtDescription = notification.Debt.Description,
                isMoney = notification.Debt.IsMoney,
                Value = notification.Value,
                Currency = notification.Debt.Currency?.CurrencySymbol,
                Message = notification.Message,
                DebtName = notification.Debt.Name,
                RowVersion = notification.RowVersion,
            };
        }

        public static PaymentDTO ToPaymentDTO(this PayNotification notification, long id)
        {
            return new PaymentDTO
            {
                CurrencyId = notification.Debt.Currency?.Id,
                Value = notification.Value,
                Date = notification.Created.Date,
                DebtName = notification.Debt.Name,
                IsOwnerPay = notification.Payer.Id == id,
                AvatarUrl = notification.Payer.Id == id
                    ? notification.UserToPay.AratarUrl
                    : notification.Payer.AratarUrl,
                Username = notification.Payer.Id == id
                    ? notification.UserToPay.Username
                    : notification.Payer.Username,
            };
        }
    }
}