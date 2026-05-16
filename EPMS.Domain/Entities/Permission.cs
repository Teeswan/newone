using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string? PermissionCode { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<PositionPermission> PositionPermissions { get; set; } = new List<PositionPermission>();
}
