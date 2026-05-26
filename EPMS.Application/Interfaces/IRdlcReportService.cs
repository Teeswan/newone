using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IRdlcReportService
{
    Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(EmployeePerformanceSummaryReportDto reportData);
    Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync();
    Task<byte[]> GenerateHighLowPerformerReportAsync();
    Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync();
    Task<byte[]> Generate360FeedbackReportAsync(AppraisalReportDto reportData);
    Task<byte[]> GeneratePerformanceAppraisalReportAsync(AppraisalReportDto reportData);
    Task<byte[]> GenerateSelfAssessmentReportAsync(AppraisalReportDto reportData);
}