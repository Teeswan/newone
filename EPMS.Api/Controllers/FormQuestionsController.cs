using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormQuestionsController : ControllerBase
{
    private readonly IFormQuestionService _service;
    private readonly IExcelPdfService _excelPdfService;

    public FormQuestionsController(IFormQuestionService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<ActionResult<IEnumerable<FormQuestionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("by-form/{formId}")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<ActionResult<IEnumerable<FormQuestionDto>>> GetByFormId(int formId)
    {
        var result = await _service.GetByFormIdAsync(formId);
        return Ok(result);
    }

    [HttpGet("{formId}/{questionId}")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<ActionResult<FormQuestionDto>> GetByFormAndQuestionId(int formId, int questionId)
    {
        var result = await _service.GetByFormAndQuestionIdAsync(formId, questionId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<ActionResult<FormQuestionDto>> Create(CreateFormQuestionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByFormAndQuestionId),
            new { formId = result.FormId, questionId = result.QuestionId }, result);
    }

    [HttpPut("{formId}/{questionId}")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<ActionResult<FormQuestionDto>> Update(int formId, int questionId, UpdateFormQuestionRequest request)
    {
        var result = await _service.UpdateAsync(formId, questionId, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{formId}/{questionId}")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<IActionResult> Delete(int formId, int questionId)
    {
        var deleted = await _service.DeleteAsync(formId, questionId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportFormQuestionsToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FormQuestions.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportFormQuestionsFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportFormQuestionsToPdfAsync();
        return File(bytes, "application/pdf", "FormQuestions.pdf");
    }
}
