using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionTitle { get; set; } = null!;

    public string? LevelId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Level? Level { get; set; }

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomeNewPositions { get; set; } = new List<PerformanceOutcome>();

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomeOldPositions { get; set; } = new List<PerformanceOutcome>();

    public virtual ICollection<PositionPermission> PositionPermissions { get; set; } = new List<PositionPermission>();
}
