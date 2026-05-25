using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;

namespace EPMS.Application.Services;

public class AccountInitializationService : IAccountInitializationService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISystemSettingRepository _systemSettingRepository;

    public AccountInitializationService(
        IPasswordHasher passwordHasher,
        IEmployeeRepository employeeRepository,
        ISystemSettingRepository systemSettingRepository)
    {
        _passwordHasher = passwordHasher;
        _employeeRepository = employeeRepository;
        _systemSettingRepository = systemSettingRepository;
    }

    public async Task InitializeAccountAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));

        if (!string.IsNullOrEmpty(employee.PasswordHash))
            return;

        if (string.IsNullOrEmpty(employee.Email))
            return;

        var hashedDefaultPassword = await _systemSettingRepository.GetValueAsync("DefaultPassword", cancellationToken);
        if (string.IsNullOrEmpty(hashedDefaultPassword))
            throw new InvalidOperationException("DefaultPassword is not set in SystemSettings.");

        employee.PasswordHash = hashedDefaultPassword;
        employee.IsFirstLogin = true;

        await _employeeRepository.UpdateAsync(employee);
    }
}
