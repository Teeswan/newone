using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceOutcomesController : ControllerBase
{
    private readonly IPerformanceOutcomeService _service;
    private readonly IExcelPdfService _excelPdfService;

    public PerformanceOutcomesController(IPerformanceOutcomeService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<ActionResult<IEnumerable<PerformanceOutcomeDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<ActionResult<PerformanceOutcomeDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("by-employee/{employeeId}")]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<ActionResult<IEnumerable<PerformanceOutcomeDto>>> GetByEmployeeId(int employeeId)
    {
        var result = await _service.GetByEmployeeIdAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("by-cycle/{cycleId}")]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<ActionResult<IEnumerable<PerformanceOutcomeDto>>> GetByCycleId(int cycleId)
    {
        var result = await _service.GetByCycleIdAsync(cycleId);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.PerformanceOutcomes.Manage)]
    public async Task<ActionResult<PerformanceOutcomeDto>> Create(CreatePerformanceOutcomeRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.OutcomeId }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.PerformanceOutcomes.Manage)]
    public async Task<ActionResult<PerformanceOutcomeDto>> Update(int id, UpdatePerformanceOutcomeRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.PerformanceOutcomes.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var bytes = await _excelPdfService.ExportPerformanceOutcomesToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PerformanceOutcomes.xlsx");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.PerformanceOutcomes.Manage)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var count = await _excelPdfService.ImportPerformanceOutcomesFromExcelAsync(stream);
        return Ok(new { Message = $"{count} records imported successfully." });
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.PerformanceOutcomes.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var bytes = await _excelPdfService.ExportPerformanceOutcomesToPdfAsync();
        return File(bytes, "application/pdf", "PerformanceOutcomes.pdf");
    }
}
