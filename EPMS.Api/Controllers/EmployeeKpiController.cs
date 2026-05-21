using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.UseCases.KpiAssignment.Commands;
using EPMS.Application.UseCases.KpiAssignment.Queries;
using EPMS.Shared.Common;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using EPMS.Application.Interfaces;

using EPMS.Shared.Enums;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/employee-kpi")]
[Authorize]
public class EmployeeKpiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IExcelPdfService _excelPdfService;

    public EmployeeKpiController(IMediator mediator, IExcelPdfService excelPdfService)
    {
        _mediator = mediator;
        _excelPdfService = excelPdfService;
    }

    [HttpGet("assignments/{employeeId}/{cycleId}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeKpiAssignmentDto>>>> GetAssignments(int employeeId, int cycleId)
    {
        var result = await _mediator.Send(new GetEmployeeKpiAssignmentQuery(employeeId, cycleId));
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<EmployeeKpiAssignmentDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<EmployeeKpiAssignmentDto>>.FailureResponse(result.Message));
    }

    [HttpGet("score-summary/{employeeId}/{cycleId}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<KpiScoreSummaryDto>>> GetScoreSummary(int employeeId, int cycleId)
    {
        var result = await _mediator.Send(new GetKpiScoreSummaryQuery(employeeId, cycleId));
        return result.IsSuccess 
            ? Ok(ApiResponse<KpiScoreSummaryDto>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<KpiScoreSummaryDto>.FailureResponse(result.Message));
    }

    [HttpGet("aggregated-summary/{cycleId}/{type}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<AggregatedKpiDto>>>> GetAggregatedSummary(int cycleId, AggregationType type)
    {
        var result = await _mediator.Send(new GetAggregatedKpiQuery(cycleId, type));
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<AggregatedKpiDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<AggregatedKpiDto>>.FailureResponse(result.Message));
    }

    [HttpPost("ad-hoc")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<decimal>>> AddAdHoc(AddAdHocKpiCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<decimal>.SuccessResponse(result.Value!, "Ad-hoc KPI added"))
            : BadRequest(ApiResponse<decimal>.FailureResponse(result.Message, result.Errors));
    }

    [HttpPost("finalise")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> Finalise(FinaliseKpiAssignmentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Assignments finalised successfully"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
    }

    [HttpPost("enter-actual")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<KpiScoreSummaryDto>>> EnterActual(EnterActualValueCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<KpiScoreSummaryDto>.SuccessResponse(result.Value!, "Actual value recorded"))
            : BadRequest(ApiResponse<KpiScoreSummaryDto>.FailureResponse(result.Message));
    }

    [HttpPost("activate-cycle-snapshot")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> ActivateSnapshot(ActivateCycleKpiSnapshotCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Cycle snapshots created"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateKpiAssignmentCommand command)
    {
        if (id != command.AssignmentId)
            return BadRequest(ApiResponse<bool>.FailureResponse("ID mismatch"));

        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Assignment updated successfully"))
            : BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteKpiAssignmentCommand(id));
        return result.IsSuccess 
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Assignment deleted successfully"))
            : BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
    }

    [HttpPost("excel-bulk-import")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<BulkImportResultDto>>> ExcelBulkImport(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<BulkImportResultDto>.FailureResponse("No file uploaded"));

        using var stream = file.OpenReadStream();
        var kpis = await _excelPdfService.ImportEmployeeKpiFromExcelAsync(stream);

        var command = new BulkImportEmployeeKpiCommand(new List<EmployeeKpiImportDto>(kpis), null);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(ApiResponse<BulkImportResultDto>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<BulkImportResultDto>.FailureResponse(result.Message, result.Errors));
    }

    [HttpGet("import-template")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<IActionResult> DownloadImportTemplate()
    {
        var fileBytes = await _excelPdfService.ExportEmployeeKpiTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_KPI_Import_Template.xlsx");
    }
}
