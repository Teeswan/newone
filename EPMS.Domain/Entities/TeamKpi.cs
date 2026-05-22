using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual DepartmentKpi DepartmentKpi { get; private set; } = null!;
        private TeamKpi() { }
    }
}
