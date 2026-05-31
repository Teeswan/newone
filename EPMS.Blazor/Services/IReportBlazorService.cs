using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public interface IReportBlazorService
{
    Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(int employeeId, int cycleId);
    Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync(int cycleId);
    Task<byte[]> GenerateHighLowPerformerReportAsync(int cycleId);
    Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync(int cycleId);
    
    // New RDLC Reports
    Task<byte[]> Generate360FeedbackRdlcAsync(int evalId);
    Task<byte[]> GeneratePerformanceAppraisalRdlcAsync(int evalId);
    Task<byte[]> GenerateSelfAssessmentRdlcAsync(int evalId);

    Task DownloadPdfAsync(byte[] pdfBytes, string fileName);
    Task OpenPdfInNewTabAsync(byte[] pdfBytes);
}