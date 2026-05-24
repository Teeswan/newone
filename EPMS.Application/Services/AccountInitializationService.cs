using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EPMS.Application.Services;

public class AccountInitializationService : IAccountInitializationService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly string _defaultPassword;

    public AccountInitializationService(
        IPasswordHasher passwordHasher,
        IEmployeeRepository employeeRepository,
        IConfiguration configuration)
    {
        _passwordHasher = passwordHasher;
        _employeeRepository = employeeRepository;
        _defaultPassword = configuration.GetValue<string>("SecuritySettings:DefaultPassword") 
            ?? throw new InvalidOperationException("SecuritySettings:DefaultPassword is missing from configuration.");
    }

    public async Task InitializeAccountAsync(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));

        if (!string.IsNullOrEmpty(employee.PasswordHash))
            return;

        if (string.IsNullOrEmpty(employee.Email))
            return;

        employee.PasswordHash = _passwordHasher.HashPassword(_defaultPassword);
        employee.IsFirstLogin = true;

        await _employeeRepository.UpdateAsync(employee);
    }
}
