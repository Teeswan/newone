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
/// Unified department-scoped access (Department → Teams → Employees).
/// </summary>
[ApiController]
[Route("api/department-scoped")]
[Authorize]
public class DepartmentScopedController : ControllerBase
{
    private readonly IDepartmentScopedManagementService _departmentScopedService;

    public DepartmentScopedController(IDepartmentScopedManagementService departmentScopedService)
    {
        _departmentScopedService = departmentScopedService;
    }

    /// <summary>
    /// Single authorization check; returns teams and employees for the current user's department.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<DepartmentScopedAccessDto>> GetScopedAccess()
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _departmentScopedService.GetScopedAccessAsync(employeeId, positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("teams")]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _departmentScopedService.GetManageableTeamsAsync(employeeId, positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("teams/{id:int}")]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<TeamDto>> GetTeamById(int id)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _departmentScopedService.GetManageableTeamByIdAsync(employeeId, positionId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPut("teams/{id:int}")]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<TeamDto>> UpdateTeam(int id, UpdateTeamRequest request)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _departmentScopedService.UpdateManageableTeamAsync(employeeId, positionId, id, request));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("employees")]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _departmentScopedService.GetViewableEmployeesAsync(employeeId, positionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("employees/{id:int}")]
    [HasPermission(Permissions.DepartmentScopedManagement)]
    public async Task<ActionResult<EmployeeDetailDto>> GetEmployeeById(int id)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _departmentScopedService.GetViewableEmployeeByIdAsync(employeeId, positionId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    private bool TryGetCurrentUserContext(out int employeeId, out int positionId)
    {
        employeeId = 0;
        positionId = 0;

        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var positionIdClaim = User.FindFirst("PositionId")?.Value;

        return employeeIdClaim != null
            && positionIdClaim != null
            && int.TryParse(employeeIdClaim, out employeeId)
            && int.TryParse(positionIdClaim, out positionId);
    }
}
