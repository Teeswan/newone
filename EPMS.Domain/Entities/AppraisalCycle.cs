using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class AppraisalCycle
{
    public int CycleId { get; set; }

    public string CycleName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? EvaluationPeriod { get; set; }

    public string? CycleStatus { get; set; }

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluations { get; set; } = new List<PerformanceEvaluation>();

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomes { get; set; } = new List<PerformanceOutcome>();
}
