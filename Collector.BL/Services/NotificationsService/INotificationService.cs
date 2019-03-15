using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Notifications;

namespace Collector.BL.Services.NotificationsService
{
    public interface INotificationService
    {
        Task<IList<NotificationReturnDTO>> GetAllNotificationsAsync();
        Task ConfirmNotificationByIdAsync(long id);
    }
}
