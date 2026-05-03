using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;

    public UserRoleService(IUserRoleRepository userRoleRepository, IMapper mapper)
    {
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
    }

    public async Task<List<UserRoleDto>> GetAllUserRolesAsync()
    {
        var userRoles = await _userRoleRepository.GetAllUserRolesAsync();
        return _mapper.Map<List<UserRoleDto>>(userRoles.ToList());
    }
}
