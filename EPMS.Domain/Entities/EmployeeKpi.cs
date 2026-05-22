using System;

namespace EPMS.Domain.Entities
{
    public class EmployeeKpi
    {
        public int EmployeeKpiId { get; private set; }
        public int EmployeeId { get; private set; }
        public int TeamKpiId { get; private set; }
        public decimal EmployeeTarget { get; private set; }
        public decimal Weight { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual Employee Employee { get; private set; } = null!;
        public virtual TeamKpi TeamKpi { get; private set; } = null!;

        private EmployeeKpi() { }

        public static EmployeeKpi Create(int employeeId, int teamKpiId, decimal employeeTarget, decimal weight)
        {
            return new EmployeeKpi
            {
                EmployeeId = employeeId,
                TeamKpiId = teamKpiId,
                EmployeeTarget = employeeTarget,
                Weight = weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(decimal employeeTarget, decimal weight)
        {
            EmployeeTarget = employeeTarget;
            Weight = weight;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
