using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.BL.Models.Notifications;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Collector.BL.Services.NotificationsService
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationService(IHttpContextAccessor httpContextAccessor,
            IRepository<Notification> notificationRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _notificationRepository = notificationRepository;
        }

        public async Task<IList<NotificationReturnDTO>> GetAllNotificationsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var notifications =
                await _notificationRepository.GetAllAsync(notification =>
                    notification.Recipient.Id == ownerId && !notification.Confirmed);

            return await notifications.Select(notification => notification.ToNotificationReturnDTO()).ToListAsync();
        }

        public async Task ConfirmNotificationByIdAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var confirmNotification =
                await _notificationRepository.GetFirstAsync(notification =>
                    notification.Id == id && !notification.Confirmed);

            if(confirmNotification == null)
                throw new ArgumentException("Notification with such id is not exists or was already confirmed");

            confirmNotification.Confirmed = true;
            await _notificationRepository.UpdateAsync(confirmNotification);
        }
    }
}