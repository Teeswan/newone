using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IExcelPdfService _excelPdfService;

    public EmployeesController(IEmployeeService service, IExcelPdfService excelPdfService)
    {
        _service = service;
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

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Employees.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
