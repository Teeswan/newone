using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EPMS.Api.Controllers;

/// <summary>
/// Global administrative access — full organization hierarchy, bypassing department scoping.
/// </summary>
[ApiController]
[Route("api/global-admin")]
[Authorize]
public class GlobalAdminController : ControllerBase
{
    private readonly IGlobalAdminManagementService _globalAdminService;

    public GlobalAdminController(IGlobalAdminManagementService globalAdminService)
    {
        _globalAdminService = globalAdminService;
    }

    /// <summary>
    /// Full organization hierarchy: all departments, teams, and employees (unscoped).
    /// </summary>
    [HttpGet("organization")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<GlobalAdminOrganizationDto>> GetOrganization()
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _globalAdminService.GetFullOrganizationAsync(positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("departments")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _globalAdminService.GetAllDepartmentsAsync(positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("teams")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _globalAdminService.GetAllTeamsAsync(positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("employees")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _globalAdminService.GetAllEmployeesAsync(positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost("departments")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentRequest request)
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _globalAdminService.CreateDepartmentAsync(positionId, request);
            return CreatedAtAction(nameof(GetDepartments), result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("departments/{id:int}")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, UpdateDepartmentRequest request)
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _globalAdminService.UpdateDepartmentAsync(positionId, id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("departments/{id:int}")]
    [HasPermission(Permissions.GlobalAdminManagement)]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        if (!TryGetPositionId(out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var deleted = await _globalAdminService.DeleteDepartmentAsync(positionId, id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    private bool TryGetPositionId(out int positionId)
    {
        positionId = 0;
        var positionIdClaim = User.FindFirst("PositionId")?.Value;
        return positionIdClaim != null && int.TryParse(positionIdClaim, out positionId);
    }
}
