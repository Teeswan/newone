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

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    // [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    // [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("code/{code}")]
    // [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<EmployeeDto>> GetByCode(string code)
    {
        var result = await _service.GetByCodeAsync(code);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("department/{departmentId}")]
    // [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartment(int departmentId)
    {
        var result = await _service.GetByDepartmentAsync(departmentId);
        return Ok(result);
    }

    [HttpGet("reports/{managerId}")]
    // [HasPermission(Permissions.Employees.View)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetDirectReports(int managerId)
    {
        var result = await _service.GetDirectReportsAsync(managerId);
        return Ok(result);
    }

    [HttpPost]
    // [HasPermission(Permissions.Employees.Manage)]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.EmployeeId }, result);
    }

    [HttpPut("{id}")]
    // [HasPermission(Permissions.Employees.Manage)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, UpdateEmployeeRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    // [HasPermission(Permissions.Employees.Manage)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
