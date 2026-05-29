using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceEvaluationsController : ControllerBase
{
    private readonly IPerformanceEvaluationService _service;
    private readonly IExcelPdfService _excelPdfService;
    private readonly IRdlcReportService _rdlcReportService;

    public PerformanceEvaluationsController(IPerformanceEvaluationService service, IExcelPdfService excelPdfService, IRdlcReportService rdlcReportService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
        _rdlcReportService = rdlcReportService;
    }

    [HttpGet]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<ActionResult<IEnumerable<PerformanceEvaluationDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<ActionResult<PerformanceEvaluationDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("by-employee/{employeeId}")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<ActionResult<IEnumerable<PerformanceEvaluationDto>>> GetByEmployeeId(int employeeId)
    {
        var result = await _service.GetByEmployeeIdAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("by-cycle/{cycleId}")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<ActionResult<IEnumerable<PerformanceEvaluationDto>>> GetByCycleId(int cycleId)
    {
        var result = await _service.GetByCycleIdAsync(cycleId);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<ActionResult<PerformanceEvaluationDto>> Create(CreatePerformanceEvaluationRequest request)
    {
        var result = await _service.CreateAsync(request, GetCurrentEmployeeId());
        return CreatedAtAction(nameof(GetById), new { id = result.EvalId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<ActionResult<PerformanceEvaluationDto>> Update(int id, UpdatePerformanceEvaluationRequest request)
    {
        var result = await _service.UpdateAsync(id, request, GetCurrentEmployeeId());
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("{id}/submit-self")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<ActionResult> SubmitSelfAssessment(int id)
    {
        var result = await _service.SubmitSelfAssessmentAsync(id, GetCurrentEmployeeId());
        if (!result) return NotFound();
        return Ok();
    }

    [HttpPost("{id}/submit-manager")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<ActionResult> SubmitManagerReview(int id)
    {
        var result = await _service.SubmitManagerReviewAsync(id, GetCurrentEmployeeId());
        if (!result) return NotFound();
        return Ok();
    }

    [HttpPost("{id}/reopen")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<ActionResult> Reopen(int id)
    {
        var result = await _service.ReopenAsync(id, GetCurrentEmployeeId());
        if (!result) return NotFound();
        return Ok();
    }

    private int? GetCurrentEmployeeId()
    {
        var claim = User.FindFirst("EmployeeId")?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportPerformanceEvaluationsToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PerformanceEvaluations.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.PerformanceEvaluations.Manage)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportPerformanceEvaluationsFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportPerformanceEvaluationsToPdfAsync();
        return File(bytes, "application/pdf", "PerformanceEvaluations.pdf");
    }

    [HttpGet("{id}/report/360")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> Export360FeedbackReport(int id, [FromQuery] string? role)
    {
        var bytes = await _excelPdfService.Export360FeedbackReportAsync(id, role);
        return File(bytes, "application/pdf", $"360Feedback_{id}.pdf");
    }

    [HttpGet("{id}/report/appraisal")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportPerformanceAppraisalReport(int id)
    {
        var bytes = await _excelPdfService.ExportPerformanceAppraisalReportAsync(id);
        return File(bytes, "application/pdf", $"PerformanceAppraisal_{id}.pdf");
    }

    [HttpGet("{id}/report/self-assessment")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportSelfAssessmentReport(int id, [FromQuery] int employeeId)
    {
        var bytes = await _excelPdfService.ExportSelfAssessmentReportAsync(id, employeeId);
        return File(bytes, "application/pdf", $"SelfAssessment_{id}.pdf");
    }

    [HttpGet("{id}/report/rdlc/360")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> Export360FeedbackRdlc(int id)
    {
        var reportData = await _service.GetAppraisalReportDataAsync(id);
        if (reportData == null) return NotFound();
        var bytes = await _rdlcReportService.Generate360FeedbackReportAsync(reportData);
        return File(bytes, "application/pdf", $"360Feedback_RDLC_{id}.pdf");
    }

    [HttpGet("{id}/report/rdlc/appraisal")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportPerformanceAppraisalRdlc(int id)
    {
        var reportData = await _service.GetAppraisalReportDataAsync(id);
        if (reportData == null) return NotFound();
        var bytes = await _rdlcReportService.GeneratePerformanceAppraisalReportAsync(reportData);
        return File(bytes, "application/pdf", $"PerformanceAppraisal_RDLC_{id}.pdf");
    }

    [HttpGet("{id}/report/rdlc/self-assessment")]
    [HasPermission(Permissions.PerformanceEvaluations.View)]
    public async Task<IActionResult> ExportSelfAssessmentRdlc(int id)
    {
        var reportData = await _service.GetAppraisalReportDataAsync(id);
        if (reportData == null) return NotFound();
        var bytes = await _rdlcReportService.GenerateSelfAssessmentReportAsync(reportData);
        return File(bytes, "application/pdf", $"SelfAssessment_RDLC_{id}.pdf");
    }
}
