using EPMS.Domain.Enums;

namespace EPMS.Shared.Requests;

public class CreatePerformanceEvaluationRequest
{
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public int FormId { get; set; }
    public PerformanceEvaluationStatus? Status { get; set; }
    public int? SelfRating { get; set; }
    public int? ManagerRating { get; set; }
    public string? SelfComments { get; set; }
    public string? ManagerComments { get; set; }
    public decimal? FinalRatingScore { get; set; }
    public bool? IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }

    public List<CreateAppraisalResponseRequest> Responses { get; set; } = new();
}

public class UpdatePerformanceEvaluationRequest
{
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public int FormId { get; set; }
    public PerformanceEvaluationStatus? Status { get; set; }
    public int? SelfRating { get; set; }
    public int? ManagerRating { get; set; }
    public string? SelfComments { get; set; }
    public string? ManagerComments { get; set; }
    public decimal? FinalRatingScore { get; set; }
    public bool? IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }

    public List<UpdateAppraisalResponseRequest> Responses { get; set; } = new();
}
