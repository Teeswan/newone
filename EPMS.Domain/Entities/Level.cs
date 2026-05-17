using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class Level
{
    public string LevelId { get; set; } = null!;

    public string? LevelName { get; set; }

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomeNewLevels { get; set; } = new List<PerformanceOutcome>();

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomeOldLevels { get; set; } = new List<PerformanceOutcome>();
}
