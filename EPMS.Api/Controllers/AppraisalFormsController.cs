using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppraisalFormsController : ControllerBase
{
    private readonly IAppraisalFormService _service;
    private readonly IExcelPdfService _excelPdfService;

    public AppraisalFormsController(IAppraisalFormService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<ActionResult<IEnumerable<AppraisalFormDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<ActionResult<AppraisalFormDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<ActionResult<AppraisalFormDto>> Create(CreateAppraisalFormRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.FormId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<ActionResult<AppraisalFormDto>> Update(int id, UpdateAppraisalFormRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportAppraisalFormsToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AppraisalForms.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.AppraisalForms.Manage)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportAppraisalFormsFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.AppraisalForms.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportAppraisalFormsToPdfAsync();
        return File(bytes, "application/pdf", "AppraisalForms.pdf");
    }
}
