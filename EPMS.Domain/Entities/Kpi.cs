using System;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;

namespace EPMS.Domain.Entities
{
    public class Kpi
    {
        public int KpiId { get; private set; }
        public string KpiName { get; private set; } = null!;
        public string? Category { get; private set; }
        public string? Unit { get; private set; }
        public bool IsActive { get; private set; }
        public KpiDirection Direction { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? CreatedByEmployeeId { get; private set; }

        private Kpi() { }

        public static Kpi Create(
            string kpiName,
            string? category,
            string? unit,
            KpiDirection direction,
            int? createdByEmployeeId)
        {
            ArgumentNullException.ThrowIfNull(kpiName);

            return new Kpi
            {
                KpiName = kpiName,
                Category = category,
                Unit = unit,
                Direction = direction,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByEmployeeId = createdByEmployeeId
            };
        }

        public void Update(
            string kpiName,
            string? category,
            string? unit,
            KpiDirection direction)
        {
            ArgumentNullException.ThrowIfNull(kpiName);

            KpiName = kpiName;
            Category = category;
            Unit = unit;
            Direction = direction;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;
    }
}