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
    }
}
