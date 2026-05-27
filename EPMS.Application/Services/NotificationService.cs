using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<NotificationDto>> GetAllNotificationsAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<NotificationDto>>(entities.ToList());
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var entities = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<List<NotificationDto>>(entities.ToList());
    }

    public async Task<NotificationDto> CreateNotificationAsync(int userId, string title, string type, int? relatedEntityId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Type = type,
            RelatedEntityId = relatedEntityId,
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        var created = await _repository.CreateAsync(notification);
        return _mapper.Map<NotificationDto>(created);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _repository.GetByIdAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _repository.UpdateAsync(notification);
        return true;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }
}
