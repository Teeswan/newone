using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppraisalQuestionsController : ControllerBase
{
    private readonly IAppraisalQuestionService _service;
    private readonly IExcelPdfService _excelPdfService;

    public AppraisalQuestionsController(IAppraisalQuestionService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.AppraisalQuestions.View)]
    public async Task<ActionResult<IEnumerable<AppraisalQuestionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.AppraisalQuestions.View)]
    public async Task<ActionResult<AppraisalQuestionDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.AppraisalQuestions.Manage)]
    public async Task<ActionResult<AppraisalQuestionDto>> Create(CreateAppraisalQuestionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.QuestionId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.AppraisalQuestions.Manage)]
    public async Task<ActionResult<AppraisalQuestionDto>> Update(int id, UpdateAppraisalQuestionRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.AppraisalQuestions.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.AppraisalQuestions.View)]                
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportAppraisalQuestionsToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AppraisalQuestions.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.AppraisalQuestions.Manage)]  
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportAppraisalQuestionsFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.AppraisalQuestions.View)]                
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportAppraisalQuestionsToPdfAsync();
        return File(bytes, "application/pdf", "AppraisalQuestions.pdf");
    }
}
