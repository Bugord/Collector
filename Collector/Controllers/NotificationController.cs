using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Services.AuthorizationService;
using Collector.BL.Services.FriendListService;
using Collector.BL.Services.NotificationsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{

    [ApiController]
    [Route("api")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("notifications")]
        [Authorize]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var data = await _notificationService.GetAllNotificationsAsync();
                return Ok(data);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPut("notifications/confirm/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmNotification(long id)
        {
            try
            {
                await _notificationService.ConfirmNotificationByIdAsync(id);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }

    
}
