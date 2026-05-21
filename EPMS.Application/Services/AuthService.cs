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
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequest request)
    {
        var employees = await _employeeRepository.GetAllAsync();
        var employee = employees.FirstOrDefault(e =>
            (!string.IsNullOrEmpty(e.Email) && e.Email.Equals(request.Identifier, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(e.Username) && e.Username.Equals(request.Identifier, StringComparison.OrdinalIgnoreCase)));

        if (employee == null)
        {
            _logger.LogWarning("Failed login attempt for identifier: {Identifier}", request.Identifier);
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

        return new LoginResponseDto
        {
            EmployeeId = employee.EmployeeId,
            FullName = employee.FullName,
            Email = employee.Email,
            Username = employee.Username,
            IsFirstLogin = employee.IsFirstLogin,
            Token = "mock-jwt-token"
        };
    }
}
