namespace EPMS.Shared.DTOs;

public class PerformanceOutcomeDto
{
    public int OutcomeId { get; set; }
    public int? EvalId { get; set; }
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public string? EmployeeName { get; set; }
    public string? CycleName { get; set; }
    public string? RecommendationType { get; set; }
    public int? OldPositionId { get; set; }
    public int? NewPositionId { get; set; }
    public string? OldLevelId { get; set; }
    public string? NewLevelId { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}
