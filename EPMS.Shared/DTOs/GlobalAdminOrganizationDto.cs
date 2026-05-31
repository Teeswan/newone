namespace EPMS.Shared.DTOs;

/// <summary>
/// Unrestricted organization hierarchy for users with GlobalAdminManagement permission.
/// </summary>
public class GlobalAdminOrganizationDto
{
    public IReadOnlyList<DepartmentDto> Departments { get; set; } = Array.Empty<DepartmentDto>();
    public IReadOnlyList<TeamDto> Teams { get; set; } = Array.Empty<TeamDto>();
    public IReadOnlyList<EmployeeDto> Employees { get; set; } = Array.Empty<EmployeeDto>();
    public GlobalAdminOrganizationSummary Summary { get; set; } = new();
}

public class GlobalAdminOrganizationSummary
{
    public int DepartmentCount { get; set; }
    public int TeamCount { get; set; }
    public int EmployeeCount { get; set; }
    public int ActiveEmployeeCount { get; set; }
}
