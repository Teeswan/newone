using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPMS.Domain.Entities
{
    public class DepartmentKpi
    {
        public int DeptKpiId { get; private set; }
        public int? DepartmentId { get; private set; }
        public int? CycleId { get; private set; }
        public int? KpiId { get; private set; }
        public decimal? DepartmentTarget { get; private set; }
        public decimal? ActualValue { get; private set; }
        public decimal? Weight { get; private set; }
        
        [NotMapped]
        public decimal Score => (DepartmentTarget ?? 0) > 0 ? ((ActualValue ?? 0) / (DepartmentTarget ?? 1)) * 100 : 0;
        
        [NotMapped]
        public decimal WeightedScore => (Score * (Weight ?? 0)) / 100;
        
        public bool? IsActive { get; private set; }
        public DateTime? CreatedAt { get; private set; }

        public virtual Department? Department { get; private set; }
        public virtual AppraisalCycle? Cycle { get; private set; }
        public virtual Kpi? Kpi { get; private set; }

        public virtual ICollection<TeamKpi> TeamKpis { get; private set; } = new List<TeamKpi>();

        private DepartmentKpi() { }

        public static DepartmentKpi Create(int departmentId, int cycleId, int kpiId, decimal departmentTarget, decimal weight, decimal actualValue = 0)
        {
            return new DepartmentKpi
            {
                DepartmentId = departmentId,
                CycleId = cycleId,
                KpiId = kpiId,
                DepartmentTarget = departmentTarget,
                Weight = weight,
                ActualValue = actualValue,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(decimal departmentTarget, decimal weight, decimal actualValue)
        {
            DepartmentTarget = departmentTarget;
            Weight = weight;
            ActualValue = actualValue;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
