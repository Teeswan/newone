using EPMS.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services
{
    public interface INotificationBlazorService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
