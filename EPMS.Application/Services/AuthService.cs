using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public AuthService(
        IEmployeeRepository employeeRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger,
        IMapper mapper,
        ITokenService tokenService)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequest request)
    {
        var employee = await _employeeRepository.GetByEmailAsync(request.Email);

        if (employee == null)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return null;
        }

        if (string.IsNullOrEmpty(employee.PasswordHash))
        {
            _logger.LogWarning("Employee {EmployeeId} has no password hash set", employee.EmployeeId);
            return null;
        }

        if (!_passwordHasher.VerifyPassword(employee.PasswordHash, request.Password))
        {
            _logger.LogWarning("Failed password verification for employee: {EmployeeId}", employee.EmployeeId);
            return null;
        }

        if (employee.IsActive != true)
        {
            _logger.LogWarning("Inactive employee attempted login: {EmployeeId}", employee.EmployeeId);
            return null;
        }

        _logger.LogInformation("Successful login for employee: {EmployeeId}", employee.EmployeeId);

        var user = await _userRepository.GetByEmployeeIdAsync(employee.EmployeeId);

        return new LoginResponseDto
        {
            EmployeeId = employee.EmployeeId,
            FullName = employee.FullName,
            Email = employee.Email,
            IsFirstLogin = employee.IsFirstLogin,
            Token = await _tokenService.CreateTokenAsync(employee, user)
        };
    }
}
