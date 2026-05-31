using EPMS.Application.Interfaces;
using EPMS.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IRdlcReportService _rdlcReportService;

    public ReportsController(IRdlcReportService rdlcReportService)
    {
        _rdlcReportService = rdlcReportService;
    }

    [HttpGet("employee-performance-summary/{employeeId}/{cycleId}")]
    public async Task<IActionResult> GenerateEmployeePerformanceSummary(int employeeId, int cycleId)
    {
        var pdfBytes = await _rdlcReportService.GenerateEmployeePerformanceSummaryReportAsync(employeeId, cycleId);
        return File(pdfBytes, "application/pdf", "EmployeePerformanceSummary.pdf");
    }

    [HttpGet("department-performance-comparison/{cycleId}")]
    public async Task<IActionResult> GenerateDepartmentPerformanceComparison(int cycleId)
    {
        var pdfBytes = await _rdlcReportService.GenerateDepartmentPerformanceComparisonReportAsync(cycleId);
        return File(pdfBytes, "application/pdf", "DepartmentPerformanceComparison.pdf");
    }

    [HttpGet("high-low-performers/{cycleId}")]
    public async Task<IActionResult> GenerateHighLowPerformerReport(int cycleId)
    {
        var pdfBytes = await _rdlcReportService.GenerateHighLowPerformerReportAsync(cycleId);
        return File(pdfBytes, "application/pdf", "HighLowPerformerReport.pdf");
    }

    [HttpGet("promotion-increment-recommendations/{cycleId}")]
    public async Task<IActionResult> GeneratePromotionIncrementRecommendation(int cycleId)
    {
        var pdfBytes = await _rdlcReportService.GeneratePromotionIncrementRecommendationReportAsync(cycleId);
        return File(pdfBytes, "application/pdf", "PromotionIncrementRecommendation.pdf");
    }
}