using EPMS.Domain.Enums;

namespace EPMS.Shared.Requests
{
    public class KpiRequest
    {
        public string KpiName { get; set; } = null!;
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public KpiDirection Direction { get; set; }
    }

    public class DepartmentKpiRequest
    {
        public int DepartmentId { get; set; }
        public int CycleId { get; set; }
        public int KpiId { get; set; }
        public decimal DepartmentTarget { get; set; }
        public decimal Weight { get; set; }
        public decimal ActualValue { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TeamKpiRequest
    {
        public int TeamId { get; set; }
        public int DeptKpiId { get; set; }
        public decimal TeamTarget { get; set; }
        public decimal Weight { get; set; }
        public decimal ActualValue { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class EmployeeKpiRequest
    {
        public int EmployeeId { get; set; }
        public int CycleId { get; set; }
        public int KpiId { get; set; }
        public int? PositionKpiId { get; set; }
        public int? TeamKpiId { get; set; }
        public decimal TargetValue { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal ActualValue { get; set; }
    }

    public class BulkEmployeeKpiRequest
    {
        public List<int> EmployeeIds { get; set; } = new();
        public int CycleId { get; set; }
        public int KpiId { get; set; }
        public int? PositionKpiId { get; set; }
        public int? TeamKpiId { get; set; }
        public decimal TargetValue { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
