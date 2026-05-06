using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PerformanceOutcome
{
    public int OutcomeId { get; set; }

    public int? EmployeeId { get; set; }

    public int? CycleId { get; set; }

    public int? EvalId { get; set; }

    public string? RecommendationType { get; set; }

    public int? OldPositionId { get; set; }

    public int? NewPositionId { get; set; }

    public string? OldLevelId { get; set; }

    public string? NewLevelId { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public virtual AppraisalCycle? Cycle { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual PerformanceEvaluation? Evaluation { get; set; }

    public virtual Level? NewLevel { get; set; }

    public virtual Position? NewPosition { get; set; }

    public virtual Level? OldLevel { get; set; }

    public virtual Position? OldPosition { get; set; }
}