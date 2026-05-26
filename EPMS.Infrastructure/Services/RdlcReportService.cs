using EPMS.Application.Interfaces;
using EPMS.Shared.DTOs;
using Microsoft.Reporting.NETCore;
using System.Reflection;

namespace EPMS.Infrastructure.Services;

public class RdlcReportService : IRdlcReportService
{
    public async Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(EmployeePerformanceSummaryReportDto reportData)
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("EmployeePerformanceSummary.rdlc");
            report.LoadReportDefinition(reportStream);

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync()
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("DepartmentPerformanceComparison.rdlc");
            report.LoadReportDefinition(reportStream);

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GenerateHighLowPerformerReportAsync()
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("HighLowPerformerReport.rdlc");
            report.LoadReportDefinition(reportStream);

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync()
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("PromotionIncrementRecommendation.rdlc");
            report.LoadReportDefinition(reportStream);

            return report.Render("PDF");
        });
    }

    private Stream GetReportStream(string reportFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"EPMS.Infrastructure.Reports.{reportFileName}";
        
        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Report file not found: {reportFileName}");
        }
        
        return stream;
    }
}

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