using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;

namespace EPMS.Application.Services;

public class AccountInitializationService : IAccountInitializationService
{
    private readonly ISystemSettingRepository _systemSettingRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmployeeRepository _employeeRepository;

    public AccountInitializationService(
        ISystemSettingRepository systemSettingRepository,
        IPasswordHasher passwordHasher,
        IEmployeeRepository employeeRepository)
    {
        _systemSettingRepository = systemSettingRepository;
        _passwordHasher = passwordHasher;
        _employeeRepository = employeeRepository;
    }

    public async Task InitializeAccountAsync(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));

        if (!string.IsNullOrEmpty(employee.PasswordHash))
            return;

        if (string.IsNullOrEmpty(employee.Email))
            return;

        var defaultPasswordHashed = await _systemSettingRepository.GetValueAsync("DefaultPassword");
        if (string.IsNullOrEmpty(defaultPasswordHashed))
            return;

        employee.Username = employee.Email;
        employee.PasswordHash = defaultPasswordHashed;
        employee.IsFirstLogin = true;

        await _employeeRepository.UpdateAsync(employee);
    }
}
