using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<RoleDto>>(entities.ToList());
    }
}
