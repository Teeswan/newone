using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EPMS.Application.Interfaces;
using EPMS.Shared.DTOs;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IExcelPdfService _excelPdfService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IExcelPdfService excelPdfService, ILogger<ReportsController> logger)
    {
        _excelPdfService = excelPdfService;
        _logger = logger;
    }

    [HttpPost("employee-performance")]
    public async Task<IActionResult> GenerateEmployeePerformanceReport([FromBody] EmployeePerformanceReportDto reportData)
    {
        try
        {
            _logger.LogInformation($"Generating performance report for employee {reportData.EmployeeCode}");
            
            var pdfBytes = await _excelPdfService.GenerateEmployeePerformanceReportAsync(reportData);
            
            var fileName = $"PerformanceReport_{reportData.EmployeeCode}_{DateTime.Now:yyyyMMdd}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating employee performance report");
            return StatusCode(500, new { Message = "Error generating report", Error = ex.Message });
        }
    }

    [HttpGet("test-sample")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSampleReport()
    {
        var sampleData = new EmployeePerformanceReportDto
        {
            EmployeeCode = "EMP001",
            FullName = "John Doe",
            PositionTitle = "Senior Developer",
            LevelName = "L4",
            DepartmentName = "Engineering",
            ReviewPeriod = "Q1 2026",
            ReviewStartDate = new DateTime(2026, 1, 1),
            ReviewEndDate = new DateTime(2026, 3, 31),
            TotalWeightedScore = 92.5m,
            AchievementPercentage = 92.5m,
            KpiCategoryScores = new List<KpiCategoryScoreDto>
            {
                new() { CategoryName = "Delivery", TotalWeight = 40, Score = 95, WeightedScore = 38 },
                new() { CategoryName = "Quality", TotalWeight = 30, Score = 90, WeightedScore = 27 },
                new() { CategoryName = "Teamwork", TotalWeight = 20, Score = 95, WeightedScore = 19 },
                new() { CategoryName = "Innovation", TotalWeight = 10, Score = 85, WeightedScore = 8.5m }
            },
            FinalRating = 4.5m,
            PerformanceLevel = "Exceeds Expectations",
            PromotionEligibility = true,
            FeedbackCriteria = new List<FeedbackCriterionDto>
            {
                new() { CriterionName = "Technical Skills", AverageScore = 4.6m, RespondentCount = 5 },
                new() { CriterionName = "Communication", AverageScore = 4.2m, RespondentCount = 5 },
                new() { CriterionName = "Leadership", AverageScore = 4.4m, RespondentCount = 5 },
                new() { CriterionName = "Problem Solving", AverageScore = 4.7m, RespondentCount = 5 },
                new() { CriterionName = "Adaptability", AverageScore = 4.3m, RespondentCount = 5 }
            },
            HasActivePip = false,
            MeetingsCompletedCount = 8,
            AppraiseeSignature = new SignatureBlockDto
            {
                SignatoryName = "John Doe",
                SignatoryRole = "Employee",
                SignedAt = new DateTime(2026, 4, 10),
                IsSigned = true
            },
            AppraiserSignature = new SignatureBlockDto
            {
                SignatoryName = "Jane Smith",
                SignatoryRole = "Engineering Manager",
                SignedAt = new DateTime(2026, 4, 12),
                IsSigned = true
            },
            HrSignature = new SignatureBlockDto
            {
                SignatoryName = "Mike Johnson",
                SignatoryRole = "HR Administrator",
                SignedAt = null,
                IsSigned = false
            }
        };

        return await GenerateEmployeePerformanceReport(sampleData);
    }
}
