using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public interface IReportBlazorService
{
    Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(EmployeePerformanceSummaryReportDto reportData);
    Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync();
    Task<byte[]> GenerateHighLowPerformerReportAsync();
    Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync();
    
    // New RDLC Reports
    Task<byte[]> Generate360FeedbackRdlcAsync(int evalId);
    Task<byte[]> GeneratePerformanceAppraisalRdlcAsync(int evalId);
    Task<byte[]> GenerateSelfAssessmentRdlcAsync(int evalId);

    Task DownloadPdfAsync(byte[] pdfBytes, string fileName);
    Task OpenPdfInNewTabAsync(byte[] pdfBytes);
}