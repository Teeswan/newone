using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IPositionPermissionRepository _repository;
    private readonly IBaseRepository<Permission, int> _permissionRepository;
    private readonly IMapper _mapper;

    public PermissionService(IPositionPermissionRepository repository, IBaseRepository<Permission, int> permissionRepository, IMapper mapper)
    {
        _repository = repository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var entities = await _permissionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(entities);
    }

    public async Task<IEnumerable<PermissionDto>> GetByPositionAsync(int positionId)
    {
        var entities = await _repository.GetPermissionsByPositionAsync(positionId);
        return _mapper.Map<IEnumerable<PermissionDto>>(entities);
    }

    public async Task<bool> AssignPermissionAsync(AssignPermissionRequest request)
    {
        var existing = await _repository.GetByPositionAndPermissionAsync(request.PositionId, request.PermissionId);
        if (existing != null)
        {
            return true;
        }

        var entity = new PositionPermission
        {
            PositionId = request.PositionId,
            PermissionId = request.PermissionId
        };

        await _repository.CreateAsync(entity);
        return true;
    }

    public async Task<bool> RevokePermissionAsync(RevokePermissionRequest request)
    {
        return await _repository.DeleteAsync(request.PositionId, request.PermissionId);
    }
}
