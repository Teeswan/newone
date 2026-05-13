using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PipObjective
{
    public int ObjectiveId { get; set; }

    public int? Pipid { get; set; }
    public string ObjectiveDescription { get; set; } = null!;
    public string? SuccessCriteria { get; set; }

    public bool? IsAchieved { get; set; }
    public string? ReviewComments { get; set; }

    public virtual PipPlan? Pip { get; set; }
}
