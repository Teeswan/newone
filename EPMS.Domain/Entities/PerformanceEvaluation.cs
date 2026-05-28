using EPMS.Domain.Enums;

namespace EPMS.Domain.Entities;

public partial class PerformanceEvaluation
{
    public int EvalId { get; set; }
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public int FormId { get; set; }
    public PerformanceEvaluationStatus Status { get; set; } = PerformanceEvaluationStatus.Draft;

    public int? SelfRating { get; set; }
    public int? ManagerRating { get; set; }
    public string? SelfComments { get; set; }
    public string? ManagerComments { get; set; }
    public string? CalibrationComments { get; set; }

    public decimal? FinalRatingScore { get; set; }
    public bool? IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }

    // --- NAVIGATION PROPERTIES ---

    public virtual ApplicationForm Form { get; set; } = null!;
    public virtual ICollection<PerformanceOutcome> PerformanceOutcomes { get; set; } = new List<PerformanceOutcome>();
    public virtual ICollection<AppraisalResponse> AppraisalResponses { get; set; } = new List<AppraisalResponse>();
    public virtual AppraisalCycle? Cycle { get; set; }
    public virtual Employee? Employee { get; set; }
}