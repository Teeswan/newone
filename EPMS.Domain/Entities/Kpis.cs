using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Entities
{
    public  class Kpis
    {
        public int KpiId { get; private set; }
        public string KpiName { get; private set; } = null!;
        public string? Category { get; private set; }
        public string? Unit { get; private set; }
        public bool IsActive { get; private set; }
        public decimal WeightPercent { get; private set; }
        public decimal? TargetValue { get; private set; }
        public int? PriorityLevel { get; private set; }
        public int? Direction { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? CreatedByEmployeeId { get; private set; }
    }
}
