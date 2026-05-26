using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IRdlcReportService
{
    Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(EmployeePerformanceSummaryReportDto reportData);
    Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync();
    Task<byte[]> GenerateHighLowPerformerReportAsync();
    Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync();
}