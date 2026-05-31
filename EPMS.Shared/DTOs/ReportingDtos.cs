namespace EPMS.Shared.DTOs;

public class DepartmentPerformanceDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal AverageScore { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
}

public class PerformerRankingDto
{
    public int Rank { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public decimal PerformanceScore { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class PromotionRecommendationDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CurrentPosition { get; set; } = string.Empty;
    public string CurrentLevel { get; set; } = string.Empty;
    public string RecommendedPosition { get; set; } = string.Empty;
    public string RecommendedLevel { get; set; } = string.Empty;
    public string RecommendationType { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public decimal PerformanceScore { get; set; }
}
