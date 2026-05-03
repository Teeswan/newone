using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users.ToList());
    }
}
