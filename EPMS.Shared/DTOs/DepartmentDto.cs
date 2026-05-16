namespace EPMS.Shared.DTOs;

public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public bool IsActive { get; set; }
    public int? ParentDepartmentId { get; set; }
    public string? ParentDepartmentName { get; set; }
}

public class DepartmentTreeDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public int? ParentDepartmentId { get; set; }
    public int Level { get; set; }
    public List<DepartmentTreeDto> Children { get; set; } = new();
}
