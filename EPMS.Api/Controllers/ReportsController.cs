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

    [HttpPost("employee-performance-summary")]
    public async Task<IActionResult> GenerateEmployeePerformanceSummary([FromBody] EmployeePerformanceSummaryReportDto reportData)
    {
        var pdfBytes = await _rdlcReportService.GenerateEmployeePerformanceSummaryReportAsync(reportData);
        return File(pdfBytes, "application/pdf", "EmployeePerformanceSummary.pdf");
    }

    [HttpGet("department-performance-comparison")]
    public async Task<IActionResult> GenerateDepartmentPerformanceComparison()
    {
        var pdfBytes = await _rdlcReportService.GenerateDepartmentPerformanceComparisonReportAsync();
        return File(pdfBytes, "application/pdf", "DepartmentPerformanceComparison.pdf");
    }

    [HttpGet("high-low-performers")]
    public async Task<IActionResult> GenerateHighLowPerformerReport()
    {
        var pdfBytes = await _rdlcReportService.GenerateHighLowPerformerReportAsync();
        return File(pdfBytes, "application/pdf", "HighLowPerformerReport.pdf");
    }

    [HttpGet("promotion-increment-recommendations")]
    public async Task<IActionResult> GeneratePromotionIncrementRecommendation()
    {
        var pdfBytes = await _rdlcReportService.GeneratePromotionIncrementRecommendationReportAsync();
        return File(pdfBytes, "application/pdf", "PromotionIncrementRecommendation.pdf");
    }
}