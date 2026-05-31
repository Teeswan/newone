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
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;
    private readonly IExcelPdfService _excelPdfService;

    public DepartmentsController(IDepartmentService service, IExcelPdfService excelPdfService)
    {
        _service = service;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.Departments.View)]                
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DepartmentDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Departments.View)]                   
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<DepartmentDto>.FailureResponse("Department not found."));
        return Ok(ApiResponse<DepartmentDto>.SuccessResponse(result));
    }

    [HttpPost]
    [HasPermission(Permissions.Departments.Manage)]                     
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create(CreateDepartmentRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.DepartmentId }, ApiResponse<DepartmentDto>.SuccessResponse(result, "Department created successfully."));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Departments.Manage)]                     
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Update(int id, UpdateDepartmentRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<DepartmentDto>.FailureResponse("Department not found."));
        return Ok(ApiResponse<DepartmentDto>.SuccessResponse(result, "Department updated successfully."));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Departments.Manage)]                
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<bool>.FailureResponse("Department not found."));
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Department deleted successfully."));
    }

    [HttpGet("tree")]
    [HasPermission(Permissions.Departments.View)]                
    public async Task<ActionResult<IEnumerable<DepartmentTreeDto>>> GetTree()
    {
        var result = await _service.GetTreeAsync();
        return Ok(result);
    }

    [HttpGet("export/excel")]
    [HasPermission(Permissions.Departments.View)]                
    public async Task<IActionResult> ExportToExcel()
    {
        var fileBytes = await _excelPdfService.ExportDepartmentsToExcelAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departments.xlsx");
    }

    [HttpGet("export/pdf")]
    [HasPermission(Permissions.Departments.View)]                
    public async Task<IActionResult> ExportToPdf()
    {
        var fileBytes = await _excelPdfService.ExportDepartmentsToPdfAsync();
        return File(fileBytes, "application/pdf", "Departments.pdf");
    }

    [HttpPost("import/excel")]
    [HasPermission(Permissions.Departments.Manage)]                
    public async Task<ActionResult<int>> ImportFromExcel(IFormFile file, [FromQuery] bool skipFirstRow = true, [FromQuery] string sheetName = "", [FromQuery] bool skipExisting = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a valid Excel file.");

        try
        {
            using var stream = file.OpenReadStream();
            var count = await _excelPdfService.ImportDepartmentsFromExcelAsync(stream, skipFirstRow, sheetName, skipExisting);
            return Ok(new { Imported = count });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
