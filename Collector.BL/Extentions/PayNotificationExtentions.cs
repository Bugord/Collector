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
                Message = notification.Message,
                DebtName = notification.Debt.Name,
                RowVersion = notification.RowVersion,
            };
        }

        public static PaymentDTO ToPaymentDTO(this PayNotification notification, long id)
        {
            return new PaymentDTO
            {
                Currency = notification.Debt.Currency?.ToCurrencyReturnDTO(),
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