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

    public async Task<byte[]> Generate360FeedbackReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("Feedback360.rdlc", reportData);
    }

    public async Task<byte[]> GeneratePerformanceAppraisalReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("PerformanceAppraisal.rdlc", reportData);
    }

    public async Task<byte[]> GenerateSelfAssessmentReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("SelfAssessment.rdlc", reportData);
    }

    private async Task<byte[]> GenerateReportAsync(string reportName, AppraisalReportDto reportData)
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream(reportName);
            report.LoadReportDefinition(reportStream);
            report.DataSources.Add(new ReportDataSource("Responses", reportData.Responses));
            report.SetParameters(new[]
            {
                new ReportParameter("EmployeeName", reportData.EmployeeName),
                new ReportParameter("EmployeeCode", reportData.EmployeeCode),
                new ReportParameter("DepartmentName", reportData.DepartmentName),
                new ReportParameter("PositionTitle", reportData.PositionTitle),
                new ReportParameter("CycleName", reportData.CycleName),
                new ReportParameter("ManagerName", reportData.ManagerName),
                new ReportParameter("AssessmentDate", reportData.AssessmentDate?.ToString("yyyy-MM-dd") ?? "N/A"),
                new ReportParameter("EffectiveDate", reportData.EffectiveDate?.ToString("yyyy-MM-dd") ?? "N/A"),
                new ReportParameter("FinalScore", reportData.FinalScore?.ToString("N2") ?? "0.00"),
                new ReportParameter("PerformanceBand", reportData.PerformanceBand),
                new ReportParameter("SelfComments", reportData.SelfComments ?? ""),
                new ReportParameter("ManagerComments", reportData.ManagerComments ?? ""),
                new ReportParameter("CalibrationComments", reportData.CalibrationComments ?? ""),
                new ReportParameter("EmployeeLevel", reportData.EmployeeLevel ?? ""),
                new ReportParameter("SelfRating", reportData.SelfRating?.ToString("N2") ?? "0.00"),
                new ReportParameter("ManagerRating", reportData.ManagerRating?.ToString("N2") ?? "0.00"),
                new ReportParameter("FinalizedByName", reportData.FinalizedByName ?? ""),
                new ReportParameter("FinalizedByDesignation", reportData.FinalizedByDesignation ?? ""),
                new ReportParameter("TotalPoints", reportData.TotalPoints.ToString()),
                new ReportParameter("AnsweredQuestionsCount", reportData.AnsweredQuestionsCount.ToString()),
                new ReportParameter("MaxPoints", (reportData.AnsweredQuestionsCount * 5).ToString())
            });

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