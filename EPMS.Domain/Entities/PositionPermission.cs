using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PositionPermission
{
    public int PositionId { get; set; }

    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Position Position { get; set; } = null!;
}
