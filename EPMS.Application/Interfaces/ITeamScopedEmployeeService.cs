using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

/// <summary>
/// Team-scoped employee management for users with <see cref="EPMS.Shared.Constants.Permissions.Employees.TeamEmployeeManagement"/>.
/// Access is limited to employees who share at least one team (TeamMembers M:N) with the current user.
/// </summary>
public interface ITeamScopedEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetManageableEmployeesAsync(int currentEmployeeId, int currentPositionId, CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto?> GetManageableEmployeeByIdAsync(int currentEmployeeId, int currentPositionId, int targetEmployeeId, CancellationToken cancellationToken = default);

    Task<EmployeeDto> UpdateManageableEmployeeAsync(int currentEmployeeId, int currentPositionId, int targetEmployeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteManageableEmployeeAsync(int currentEmployeeId, int currentPositionId, int targetEmployeeId, CancellationToken cancellationToken = default);
}
