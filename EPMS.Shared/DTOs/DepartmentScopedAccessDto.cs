namespace EPMS.Shared.DTOs;

/// <summary>
/// Unified department-scoped access payload: teams in the department and employees on those teams.
/// </summary>
public class DepartmentScopedAccessDto
{
    public int DepartmentId { get; set; }
    public IReadOnlyList<TeamDto> Teams { get; set; } = Array.Empty<TeamDto>();
    public IReadOnlyList<EmployeeDto> Employees { get; set; } = Array.Empty<EmployeeDto>();
}
