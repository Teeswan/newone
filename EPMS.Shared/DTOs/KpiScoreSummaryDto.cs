using System.Collections.Generic;

namespace EPMS.Shared.DTOs;

public class KpiScoreSummaryDto
{
    public int EmployeeId { get; set; }
    public int CycleId { get; set; }
    public decimal TotalPerformanceScore { get; set; }
    public int Rating { get; set; }
    public string PerformanceLabel { get; set; } = string.Empty;
    public string PromotionEligibility { get; set; } = string.Empty;
    public List<EmployeeKpiAssignmentDto> Assignments { get; set; } = new();
}
