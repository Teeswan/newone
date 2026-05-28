using EPMS.Domain.Enums;

namespace EPMS.Shared.DTOs;

public class PerformanceEvaluationDto
{
    public int EvalId { get; set; }
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public int FormId { get; set; }
    public AppraisalFormType FormType { get; set; }
    public string? EmployeeName { get; set; }
    public string? CycleName { get; set; }
    public string? FormName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionTitle { get; set; }
    public string? ManagerName { get; set; }
    public int? CreatedByEmployeeId { get; set; }
    public string? CreatedByName { get; set; }
    public PerformanceEvaluationStatus Status { get; set; }
    public int? SelfRating { get; set; }
    public int? ManagerRating { get; set; }
    public string? SelfComments { get; set; }
    public string? ManagerComments { get; set; }
    public string? CalibrationComments { get; set; }
    public decimal? FinalRatingScore { get; set; }
    public bool? IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }

    public List<AppraisalResponseDto> Responses { get; set; } = new();
    public List<FormQuestionDto> FormQuestions { get; set; } = new();
}
