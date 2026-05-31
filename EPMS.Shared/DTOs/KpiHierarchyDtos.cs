using EPMS.Domain.Enums;
using System;

namespace EPMS.Shared.DTOs
{
    public class KpiDto
    {
        public int KpiId { get; set; }
        public string KpiName { get; set; } = null!;
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; }
        public KpiDirection Direction { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DepartmentKpiDto
    {
        public int DeptKpiId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? CycleId { get; set; }
        public string? CycleName { get; set; }
        public int? KpiId { get; set; }
        public string? KpiName { get; set; }
        public decimal? DepartmentTarget { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Score { get; set; }
        public decimal? WeightedScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class TeamKpiDto
    {
        public int TeamKpiId { get; set; }
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public int DeptKpiId { get; set; }
        public string? KpiName { get; set; }
        public decimal TeamTarget { get; set; }
        public decimal ParentTarget { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
        public decimal WeightedScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeKpiDto
    {
        public int EmployeeKpiId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public int KpiId { get; set; }
        public string? KpiName { get; set; }
        public int? PositionKpiId { get; set; }
        public int? TeamKpiId { get; set; }
        public decimal TargetValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
        public decimal WeightedScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

   
}
