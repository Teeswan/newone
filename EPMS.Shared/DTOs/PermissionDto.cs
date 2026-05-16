namespace EPMS.Shared.DTOs;

public class PermissionDto
{
    public int PermissionId { get; set; }
    public string PermissionCode { get; set; } = null!;
    public string? Description { get; set; }
}

public class PositionPermissionDto
{
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public int PermissionId { get; set; }
    public string? PermissionCode { get; set; }
}
