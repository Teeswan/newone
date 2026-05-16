namespace EPMS.Shared.Requests;

public class CreateAppraisalCycleRequest
{
    public string CycleName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? EvaluationPeriod { get; set; }
    public string? CycleStatus { get; set; }
}

public class UpdateAppraisalCycleRequest
{
    public string CycleName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? EvaluationPeriod { get; set; }
    public string? CycleStatus { get; set; }
}
