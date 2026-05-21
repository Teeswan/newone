using MediatR;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.UseCases.Auth.Commands;

public class ChangePasswordCommand : IRequest<bool>
{
    public int EmployeeId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdFromDbAsync(request.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Change password failed: Employee {EmployeeId} not found", request.EmployeeId);
            return false;
        }

        employee.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        employee.IsFirstLogin = false;

        await _employeeRepository.UpdateAsync(employee);

        _logger.LogInformation("Password changed successfully for employee: {EmployeeId}", request.EmployeeId);
        return true;
    }
}
