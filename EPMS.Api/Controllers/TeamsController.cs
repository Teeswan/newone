using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Common;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _service;
    private readonly IExcelPdfService _excelPdfService;

    public TeamsController(ITeamService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.Teams.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TeamDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Teams.View)]
    public async Task<ActionResult<ApiResponse<TeamDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<TeamDto>.FailureResponse("Team not found."));
        return Ok(ApiResponse<TeamDto>.SuccessResponse(result));
    }

    [HttpPost]
    [HasPermission(Permissions.Teams.Manage)]
    public async Task<ActionResult<ApiResponse<TeamDto>>> Create(CreateTeamRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.TeamId }, ApiResponse<TeamDto>.SuccessResponse(result, "Team created successfully."));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Teams.Manage)]
    public async Task<ActionResult<ApiResponse<TeamDto>>> Update(int id, UpdateTeamRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<TeamDto>.FailureResponse("Team not found."));
        return Ok(ApiResponse<TeamDto>.SuccessResponse(result, "Team updated successfully."));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Teams.Manage)]                
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<bool>.FailureResponse("Team not found."));
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Team deleted successfully."));
    }

    [HttpGet("department/{departmentId}")]
    [HasPermission(Permissions.Teams.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDetailDto>>>> GetByDepartment(int departmentId)
    {
        var result = await _service.GetByDepartmentAsync(departmentId);
        return Ok(ApiResponse<IEnumerable<TeamDetailDto>>.SuccessResponse(result));
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.Teams.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var fileBytes = await _excelPdfService.ExportTeamsToExcelAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Teams.xlsx");
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.Teams.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var fileBytes = await _excelPdfService.ExportTeamsToPdfAsync();
        return File(fileBytes, "application/pdf", "Teams.pdf");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.Teams.Manage)]
    public async Task<ActionResult<int>> ImportFromExcel(IFormFile file, [FromQuery] bool skipFirstRow = true, [FromQuery] string sheetName = "", [FromQuery] bool skipExisting = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a valid Excel file.");

        try
        {
            using var stream = file.OpenReadStream();
            var count = await _excelPdfService.ImportTeamsFromExcelAsync(stream, skipFirstRow, sheetName, skipExisting);
            return Ok(new { Imported = count });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
