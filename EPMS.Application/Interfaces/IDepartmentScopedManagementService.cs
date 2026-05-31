using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

/// <summary>
/// Unified department-scoped access (Department → Teams → Employees).
/// Requires <see cref="EPMS.Shared.Constants.Permissions.DepartmentScopedManagement"/>.
/// </summary>
public interface IDepartmentScopedManagementService
{
    /// <summary>
    /// Single authorization check, then returns teams and employees for the current user's department.
    /// </summary>
    Task<DepartmentScopedAccessDto> GetScopedAccessAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TeamDto>> GetManageableTeamsAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<TeamDto?> GetManageableTeamByIdAsync(
        int currentEmployeeId,
        int currentPositionId,
        int teamId,
        CancellationToken cancellationToken = default);

    Task<TeamDto> UpdateManageableTeamAsync(
        int currentEmployeeId,
        int currentPositionId,
        int teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeDto>> GetViewableEmployeesAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto?> GetViewableEmployeeByIdAsync(
        int currentEmployeeId,
        int currentPositionId,
        int targetEmployeeId,
        CancellationToken cancellationToken = default);
}
