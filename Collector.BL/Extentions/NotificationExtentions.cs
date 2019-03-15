using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Notifications;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class NotificationExtentions
    {
        public static NotificationReturnDTO ToNotificationReturnDTO(this Notification notification)
        {
            return new NotificationReturnDTO
            {
                Id = notification.Id,
                Message = notification.Message
            };
        }
    }
}
