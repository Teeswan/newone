using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Entities
{
    public class DepartmentKpi
    {
        public int DeptKpiId { get; private set; }
        public int DepartmentId { get; private set; }
        public int CycleId { get; private set; }
        public int KpiMasterId { get; private set; }
        public decimal DepartmentTarget { get; private set; }
        public decimal Weight { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual Kpis KpiMaster { get; private set; } = null!;
        private DepartmentKpi() { }
    }
}
