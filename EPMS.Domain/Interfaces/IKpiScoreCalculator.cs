using System.Collections.Generic;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;

namespace EPMS.Domain.Interfaces;

public interface IKpiScoreCalculator
{
    decimal CalculateKpiScore(decimal actual, decimal target, KpiDirection direction);
    decimal CalculateWeightedScore(decimal kpiScore, decimal weightPercent);
    decimal CalculateTotalScore(IEnumerable<EmployeeKpiAssignment> assignments);
    (int rating, string label, string promotion) MapToRating(decimal totalScore);
}
