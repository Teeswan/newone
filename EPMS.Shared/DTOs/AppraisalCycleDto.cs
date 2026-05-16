namespace EPMS.Shared.DTOs;

public class AppraisalCycleDto
{
    public int CycleId { get; set; }
    public string CycleName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? EvaluationPeriod { get; set; }
    public string? CycleStatus { get; set; }
}
