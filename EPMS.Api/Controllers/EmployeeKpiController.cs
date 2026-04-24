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

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/employee-kpi")]
[Authorize]
public class EmployeeKpiController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeKpiController(IMediator mediator)
    {
        _mediator = mediator;
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
}
