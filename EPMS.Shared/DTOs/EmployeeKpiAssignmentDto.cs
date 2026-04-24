using EPMS.Domain.Enums;

namespace EPMS.Shared.DTOs;

public class EmployeeKpiAssignmentDto
{
    public int AssignmentId { get; set; }
    public int EmployeeId { get; set; }
    public int CycleId { get; set; }
    public string KpiNameSnapshot { get; set; } = null!;
    public string? CategorySnapshot { get; set; }
    public string? UnitSnapshot { get; set; }
    public KpiDirection Direction { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal TargetValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? KpiScore { get; set; }
    public decimal? WeightedScore { get; set; }
    public KpiAssignmentStatus Status { get; set; }
    public bool IsAdHoc { get; set; }
    public decimal CategoryWeightSubtotal { get; set; }
}
