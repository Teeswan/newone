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
            ArgumentNullException.ThrowIfNull(kpiName);

            ValidateWeightBand(priorityLevel, weightPercent);

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
            ArgumentNullException.ThrowIfNull(kpiName);

            ValidateWeightBand(priorityLevel, weightPercent);

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

        private static void ValidateWeightBand(
            PriorityLevel priority,
            decimal weight)
        {
            bool isValid = priority switch
            {
                PriorityLevel.Critical => weight >= 20 && weight <= 35,
                PriorityLevel.High => weight >= 10 && weight <= 15,
                PriorityLevel.Medium => Math.Abs(weight - 10) < 0.01m,
                PriorityLevel.Lower => Math.Abs(weight - 5) < 0.01m,
                _ => false
            };

            if (!isValid)
            {
                throw new KpiWeightBandException(
                    $"Invalid weight {weight}% for priority {priority}. " +
                    "Rules: Critical 20-35%, High 10-15%, Medium 10%, Lower 5%.");
            }
        }
    }
}