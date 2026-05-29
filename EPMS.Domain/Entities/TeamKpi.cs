using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities
{
    public class TeamKpi
    {
        public int TeamKpiId { get; private set; }
        public int TeamId { get; private set; }
        public int DeptKpiId { get; private set; }
        public decimal TeamTarget { get; private set; }
        public decimal Weight { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual Team Team { get; private set; } = null!;
        public virtual DepartmentKpi DepartmentKpi { get; private set; } = null!;

        public virtual ICollection<EmployeeKpi> EmployeeKpis { get; private set; } = new List<EmployeeKpi>();

        private TeamKpi() { }

        public static TeamKpi Create(int teamId, int deptKpiId, decimal teamTarget, decimal weight)
        {
            return new TeamKpi
            {
                TeamId = teamId,
                DeptKpiId = deptKpiId,
                TeamTarget = teamTarget,
                Weight = weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(decimal teamTarget, decimal weight)
        {
            TeamTarget = teamTarget;
            Weight = weight;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
