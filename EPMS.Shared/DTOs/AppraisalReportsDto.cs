namespace EPMS.Shared.DTOs;

public class AppraisalReportDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string CycleName { get; set; } = string.Empty;
    public DateTime? AssessmentDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public decimal? FinalScore { get; set; }
    public string PerformanceBand { get; set; } = string.Empty;
    public List<AppraisalResponseDto> Responses { get; set; } = new();
}
