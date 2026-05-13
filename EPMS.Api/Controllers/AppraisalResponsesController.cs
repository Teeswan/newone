using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppraisalResponsesController : ControllerBase
{
    private readonly IAppraisalResponseService _service;
    private readonly IExcelPdfService _excelPdfService;

    public AppraisalResponsesController(IAppraisalResponseService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.AppraisalResponses.View)]                
    public async Task<ActionResult<IEnumerable<AppraisalResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.AppraisalResponses.View)]                
    public async Task<ActionResult<AppraisalResponseDto>> GetById(long id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("by-evaluation/{evalId}")]
    [HasPermission(Permissions.AppraisalResponses.View)]                
    public async Task<ActionResult<IEnumerable<AppraisalResponseDto>>> GetByEvalId(int evalId)
    {
        var result = await _service.GetByEvalIdAsync(evalId);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.AppraisalResponses.Manage)]                
    public async Task<ActionResult<AppraisalResponseDto>> Create(CreateAppraisalResponseRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.ResponseId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.AppraisalResponses.Manage)]                
    public async Task<ActionResult<AppraisalResponseDto>> Update(long id, UpdateAppraisalResponseRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.AppraisalResponses.Manage)]                
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.AppraisalResponses.View)]                
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportAppraisalResponsesToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AppraisalResponses.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.AppraisalResponses.Manage)]                
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportAppraisalResponsesFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.AppraisalResponses.View)]                
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportAppraisalResponsesToPdfAsync();
        return File(bytes, "application/pdf", "AppraisalResponses.pdf");
    }
}
