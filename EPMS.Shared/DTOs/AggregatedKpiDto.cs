namespace EPMS.Shared.DTOs;

public class AggregatedKpiDto
{
    public int EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public decimal AverageScore { get; set; }
    public int EmployeeCount { get; set; }
    public int CycleId { get; set; }
    public string CycleName { get; set; } = string.Empty;
}
