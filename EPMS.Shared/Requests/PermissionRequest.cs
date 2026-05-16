namespace EPMS.Shared.Requests;

public class AssignPermissionRequest
{
    public int PositionId { get; set; }
    public int PermissionId { get; set; }
}

public class RevokePermissionRequest
{
    public int PositionId { get; set; }
    public int PermissionId { get; set; }
}
