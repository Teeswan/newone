using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

/// <summary>
/// Global administrative access across the full organization hierarchy.
/// Requires <see cref="EPMS.Shared.Constants.Permissions.GlobalAdminManagement"/>.
/// Bypasses department-level scoping used by standard managers.
/// </summary>
public interface IGlobalAdminManagementService
{
    /// <summary>
    /// Returns the complete organization: all departments, teams, and distinct employees.
    /// </summary>
    Task<GlobalAdminOrganizationDto> GetFullOrganizationAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TeamDto>> GetAllTeamsAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateDepartmentAsync(
        int currentPositionId,
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> UpdateDepartmentAsync(
        int currentPositionId,
        int departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteDepartmentAsync(
        int currentPositionId,
        int departmentId,
        CancellationToken cancellationToken = default);
}
