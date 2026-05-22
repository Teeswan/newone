using System;
using System.Collections.Generic;
using EPMS.Domain.Enums;

namespace EPMS.Domain.Entities
{
    public class Kpi
    {
        public int KpiId { get; private set; }
        public string KpiName { get; private set; } = null!;
        public string? Category { get; private set; }
        public string? Unit { get; private set; }
        public bool IsActive { get; private set; }
        public decimal WeightPercent { get; private set; }
        public decimal? TargetValue { get; private set; }
        public PriorityLevel PriorityLevel { get; private set; }
        public KpiDirection Direction { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? CreatedByEmployeeId { get; private set; }

        private Kpi() { }

        public static Kpi Create(
            string kpiName,
            string? category,
            string? unit,
            decimal weightPercent,
            decimal? targetValue,
            PriorityLevel priorityLevel,
            KpiDirection direction,
            int? createdByEmployeeId)
        {
            return new Kpi
            {
                KpiName = kpiName,
                Category = category,
                Unit = unit,
                WeightPercent = weightPercent,
                TargetValue = targetValue,
                PriorityLevel = priorityLevel,
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
            decimal weightPercent,
            decimal? targetValue,
            PriorityLevel priorityLevel,
            KpiDirection direction)
        {
            KpiName = kpiName;
            Category = category;
            Unit = unit;
            WeightPercent = weightPercent;
            TargetValue = targetValue;
            PriorityLevel = priorityLevel;
            Direction = direction;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
