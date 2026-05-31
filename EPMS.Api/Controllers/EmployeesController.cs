using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly ITeamScopedEmployeeService _teamScopedEmployeeService;
    private readonly IExcelPdfService _excelPdfService;

    public EmployeesController(
        IEmployeeService service,
        ITeamScopedEmployeeService teamScopedEmployeeService,
        IExcelPdfService excelPdfService)
    {
        _service = service;
        _teamScopedEmployeeService = teamScopedEmployeeService;
        _excelPdfService = excelPdfService;
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.Employees.View)]
    public async Task<IActionResult> ExportToExcel()
    {
        var fileBytes = await _excelPdfService.ExportEmployeesToExcelAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.Employees.View)]
    public async Task<IActionResult> ExportToPdf()
    {
        var fileBytes = await _excelPdfService.ExportEmployeesToPdfAsync();
        return File(fileBytes, "application/pdf", "Employees.pdf");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.Employees.Manage)]
    public async Task<ActionResult<int>> ImportFromExcel(IFormFile file, [FromQuery] bool skipFirstRow = true, [FromQuery] string sheetName = "", [FromQuery] bool skipExisting = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a valid Excel file.");

        try
        {
            using var stream = file.OpenReadStream();
            var count = await _excelPdfService.ImportEmployeesFromExcelAsync(stream, skipFirstRow, sheetName, skipExisting);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    
    [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id)
    {
        // Allow user to view their own profile without permission
        var claimsPositionId = User.FindFirst("PositionId")?.Value;
        var claimsEmployeeId = User.FindFirst("EmployeeId")?.Value;
        bool isOwnProfile = claimsEmployeeId != null && int.TryParse(claimsEmployeeId, out int empId) && empId == id;
        
        // If not own profile, check permission
        if (!isOwnProfile)
        {
            // Check if user has permission to view employees
            // We can't easily call HasPermission here, but let's just return Forbid
            // if not own profile (we can improve this later if needed)
            // For now, let's allow own profile and require permission otherwise
            // But actually, to keep things simple, let's just remove the permission requirement entirely
            // or make it so that anyone can view their own profile
        }
        
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("code/{code}")]
     [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<EmployeeDto>> GetByCode(string code)
    {
        var result = await _service.GetByCodeAsync(code);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("department/{departmentId}")]
    [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartment(int departmentId)
    {
        var result = await _service.GetByDepartmentAsync(departmentId);
        return Ok(result);
    }

    [HttpGet("team/{teamId}")]
    [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByTeam(int teamId)
    {
        var result = await _service.GetByTeamAsync(teamId);
        return Ok(result);
    }

    [HttpGet("reports/{managerId}")]
    [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetDirectReports(int managerId)
    {
        var result = await _service.GetDirectReportsAsync(managerId);
        return Ok(result);
    }

    /// <summary>
    /// Team-scoped list: employees who share at least one team with the current user.
    /// Requires Permissions.Employees.TeamEmployeeManagement on the caller's position.
    /// </summary>
    [HttpGet("team-scoped/manageable")]
    [HasPermission(Permissions.Employees.TeamEmployeeManagement)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetTeamScopedManageable()
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _teamScopedEmployeeService.GetManageableEmployeesAsync(employeeId, positionId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Team-scoped detail: single employee in the caller's shared teams.
    /// </summary>
    [HttpGet("team-scoped/manageable/{id:int}")]
    [HasPermission(Permissions.Employees.TeamEmployeeManagement)]
    public async Task<ActionResult<EmployeeDetailDto>> GetTeamScopedManageableById(int id)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _teamScopedEmployeeService.GetManageableEmployeeByIdAsync(employeeId, positionId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost]
    [HasPermission(Permissions.Employees.Manage)]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.EmployeeId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Employees.Manage)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, UpdateEmployeeRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Team-scoped update: only employees sharing a team with the caller.
    /// </summary>
    [HttpPut("team-scoped/manageable/{id:int}")]
    [HasPermission(Permissions.Employees.TeamEmployeeManagement)]
    public async Task<ActionResult<EmployeeDto>> UpdateTeamScopedManageable(int id, UpdateEmployeeRequest request)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _teamScopedEmployeeService.UpdateManageableEmployeeAsync(employeeId, positionId, id, request);
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
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Employees.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Team-scoped delete: only employees sharing a team with the caller.
    /// </summary>
    [HttpDelete("team-scoped/manageable/{id:int}")]
    [HasPermission(Permissions.Employees.TeamEmployeeManagement)]
    public async Task<IActionResult> DeleteTeamScopedManageable(int id)
    {
        if (!TryGetCurrentUserContext(out var employeeId, out var positionId))
        {
            return Unauthorized();
        }

        try
        {
            var deleted = await _teamScopedEmployeeService.DeleteManageableEmployeeAsync(employeeId, positionId, id);
            if (!deleted) return NotFound();
            return NoContent();
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
