using EPMS.Domain.Enums;

namespace EPMS.Shared.DTOs;

public class KpiMasterDto
{
    public int KpiId { get; set; }
    public string KpiName { get; set; } = null!;
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal? TargetValue { get; set; }
    public PriorityLevel PriorityLevel { get; set; }
    public KpiDirection Direction { get; set; }
    public int? PositionId { get; set; }
    public bool IsActive { get; set; }
}
