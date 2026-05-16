using System;
using System.Collections.Generic;
using System.Linq;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;

namespace EPMS.Domain.Services;

public class KpiScoreCalculator : IKpiScoreCalculator
{
    public decimal CalculateKpiScore(decimal actual, decimal target, KpiDirection direction)
    {
        if (target == 0) return 0;

        decimal score;
        if (direction == KpiDirection.HigherIsBetter)
        {
            score = (actual / target) * 100;
        }
        else // LowerIsBetter
        {
            if (actual == 0) return 100; // Guard against division by zero if actual is 0
            score = (target / actual) * 100;
            if (score > 100) score = 100; // Capped at 100% for LowerIsBetter as per rules
        }

        return Math.Round(score, 2);
    }

    public decimal CalculateWeightedScore(decimal kpiScore, decimal weightPercent)
    {
        return Math.Round(kpiScore * (weightPercent / 100), 2);
    }

    public decimal CalculateTotalScore(IEnumerable<EmployeeKpiAssignment> assignments)
    {
        return assignments.Sum(a => a.WeightedScore ?? 0);
    }

    public (int rating, string label, string promotion) MapToRating(decimal totalScore)
    {
        // score >= 86 → Rating 5 (Excellent/Outstanding) → PromotionEligibility: Strongly Recommended
        // score >= 71 → Rating 4 (Good)                  → Eligible
        // score >= 60 → Rating 3 (Meets Requirement)     → Possible
        // score >= 40 → Rating 2 (Needs Improvement)     → Not Eligible
        // else        → Rating 1 (Unsatisfactory)        → Not Eligible

        if (totalScore >= 86) return (5, "Excellent/Outstanding", "Strongly Recommended");
        if (totalScore >= 71) return (4, "Good", "Eligible");
        if (totalScore >= 60) return (3, "Meets Requirement", "Possible");
        if (totalScore >= 40) return (2, "Needs Improvement", "Not Eligible");
        return (1, "Unsatisfactory", "Not Eligible");
    }
}
