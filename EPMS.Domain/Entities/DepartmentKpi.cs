using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities
{
    public class DepartmentKpi
    {
        public int DeptKpiId { get; private set; }
        public int DepartmentId { get; private set; }
        public int CycleId { get; private set; }
        public int KpiId { get; private set; }
        public decimal DepartmentTarget { get; private set; }
        public decimal Weight { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual Department Department { get; private set; } = null!;
        public virtual AppraisalCycle Cycle { get; private set; } = null!;
        public virtual Kpi Kpi { get; private set; } = null!;

        private DepartmentKpi() { }

        public static DepartmentKpi Create(int departmentId, int cycleId, int kpiId, decimal departmentTarget, decimal weight)
        {
            return new DepartmentKpi
            {
                DepartmentId = departmentId,
                CycleId = cycleId,
                KpiId = kpiId,
                DepartmentTarget = departmentTarget,
                Weight = weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(decimal departmentTarget, decimal weight)
        {
            DepartmentTarget = departmentTarget;
            Weight = weight;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
