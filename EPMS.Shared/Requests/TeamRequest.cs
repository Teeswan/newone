namespace EPMS.Shared.Requests;

public class CreateTeamRequest
{
    public string TeamName { get; set; } = null!;
    public int? ManagerId { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateTeamRequest
{
    public string TeamName { get; set; } = null!;
    public int? ManagerId { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}
