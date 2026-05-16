namespace EPMS.Shared.DTOs;

public class TeamDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}

public class TeamDetailDto : TeamDto
{
    public int MemberCount { get; set; }
}
