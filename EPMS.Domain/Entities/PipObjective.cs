using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PipObjective
{
    public int ObjectiveId { get; set; }

    public int? Pipid { get; set; }

    public bool? IsAchieved { get; set; }

    public virtual PipPlan? Pip { get; set; }
}
