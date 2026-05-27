using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification, int>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}