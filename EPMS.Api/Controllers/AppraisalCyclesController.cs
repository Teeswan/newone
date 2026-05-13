using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppraisalCyclesController : ControllerBase
{
    private readonly IAppraisalCycleService _service;
    private readonly IExcelPdfService _excelPdfService;

    public AppraisalCyclesController(IAppraisalCycleService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.AppraisalCycles.View)]
    public async Task<ActionResult<IEnumerable<AppraisalCycleDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.AppraisalCycles.View)]
    public async Task<ActionResult<AppraisalCycleDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.AppraisalCycles.Manage)]
    public async Task<ActionResult<AppraisalCycleDto>> Create(CreateAppraisalCycleRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.CycleId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.AppraisalCycles.Manage)]
    public async Task<ActionResult<AppraisalCycleDto>> Update(int id, UpdateAppraisalCycleRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.AppraisalCycles.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.AppraisalCycles.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportAppraisalCyclesToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AppraisalCycles.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.AppraisalCycles.Manage)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportAppraisalCyclesFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.AppraisalCycles.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportAppraisalCyclesToPdfAsync();
        return File(bytes, "application/pdf", "AppraisalCycles.pdf");
    }
}
