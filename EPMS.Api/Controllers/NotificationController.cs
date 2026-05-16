using EPMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();

            if (notifications == null || notifications.Count == 0)
            {
                return NotFound("No notifications found.");
            }

            return Ok(notifications);
        }
    }
}
