using EPMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAllNotificationsAsync();
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<NotificationDto> CreateNotificationAsync(int userId, string title, string type, int? relatedEntityId = null);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
