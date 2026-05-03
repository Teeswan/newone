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
}
