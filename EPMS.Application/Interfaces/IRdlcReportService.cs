using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IRdlcReportService
{
    Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(int employeeId, int cycleId);
    Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync(int cycleId);
    Task<byte[]> GenerateHighLowPerformerReportAsync(int cycleId);
    Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync(int cycleId);
    Task<byte[]> Generate360FeedbackReportAsync(AppraisalReportDto reportData);
    Task<byte[]> GeneratePerformanceAppraisalReportAsync(AppraisalReportDto reportData);
    Task<byte[]> GenerateSelfAssessmentReportAsync(AppraisalReportDto reportData);
}