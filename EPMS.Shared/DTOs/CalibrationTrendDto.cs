namespace EPMS.Shared.DTOs;

public class CalibrationTrendDto
{
    public int ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public int TotalEvaluations { get; set; }
    public int AdjustedCount { get; set; }
    public decimal AverageAdjustment { get; set; }
    public string TrendStatus { get; set; } = string.Empty; // e.g., "High-Bias", "Consistent"
}
