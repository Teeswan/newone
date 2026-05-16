namespace EPMS.Shared.Requests;

public class CreateDepartmentRequest
{
    public string DepartmentName { get; set; } = null!;
    public int? ParentDepartmentId { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class UpdateDepartmentRequest
{
    public string DepartmentName { get; set; } = null!;
    public int? ParentDepartmentId { get; set; }
    public bool? IsActive { get; set; }
}
