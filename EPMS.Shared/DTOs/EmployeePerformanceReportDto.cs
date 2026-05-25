using System;
using System.Collections.Generic;

namespace EPMS.Shared.DTOs;

public class EmployeePerformanceReportDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string ReviewPeriod { get; set; } = string.Empty;
    public DateTime ReviewStartDate { get; set; }
    public DateTime ReviewEndDate { get; set; }

    public decimal TotalWeightedScore { get; set; }
    public decimal AchievementPercentage { get; set; }
    public List<KpiCategoryScoreDto> KpiCategoryScores { get; set; } = new();

    public decimal FinalRating { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty;
    public bool PromotionEligibility { get; set; }

    public List<FeedbackCriterionDto> FeedbackCriteria { get; set; } = new();

    public bool HasActivePip { get; set; }
    public string? PipStatus { get; set; }
    public string? CurrentOutcomeState { get; set; }

    public int MeetingsCompletedCount { get; set; }

    public SignatureBlockDto AppraiseeSignature { get; set; } = new();
    public SignatureBlockDto AppraiserSignature { get; set; } = new();
    public SignatureBlockDto HrSignature { get; set; } = new();
}

public class KpiCategoryScoreDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal Score { get; set; }
    public decimal WeightedScore { get; set; }
}

public class FeedbackCriterionDto
{
    public string CriterionName { get; set; } = string.Empty;
    public decimal AverageScore { get; set; }
    public int RespondentCount { get; set; }
}

public class SignatureBlockDto
{
    public string SignatoryName { get; set; } = string.Empty;
    public string SignatoryRole { get; set; } = string.Empty;
    public DateTime? SignedAt { get; set; }
    public bool IsSigned { get; set; }
}
