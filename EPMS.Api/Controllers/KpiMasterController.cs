using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.UseCases.KpiMaster.Commands;
using EPMS.Application.UseCases.KpiMaster.Queries;
using EPMS.Shared.Common;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/kpi-master")]
[Authorize]
public class KpiMasterController : ControllerBase
{
    private readonly IMediator _mediator;

    public KpiMasterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<KpiMasterDto>>>> GetList([FromQuery] GetKpiMasterListQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<KpiMasterDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<KpiMasterDto>>.FailureResponse(result.Message, result.Errors));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<KpiMasterDto>>> GetById(int id)
    {
        var result = await _mediator.Send(new GetKpiMasterByIdQuery(id));
        return result.IsSuccess 
            ? Ok(ApiResponse<KpiMasterDto>.SuccessResponse(result.Value!))
            : NotFound(ApiResponse<KpiMasterDto>.FailureResponse(result.Message));
    }

    [HttpGet("by-position/{positionId}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<KpiMasterDto>>>> GetByPosition(int positionId)
    {
        var result = await _mediator.Send(new GetKpisByPositionQuery(positionId));
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<KpiMasterDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<KpiMasterDto>>.FailureResponse(result.Message));
    }

    [HttpPost]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateKpiMasterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, ApiResponse<int>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<int>.FailureResponse(result.Message, result.Errors));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdateKpiMasterCommand command)
    {
        if (id != command.KpiId) return BadRequest(ApiResponse<object>.FailureResponse("ID mismatch"));
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Updated successfully"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message, result.Errors));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(int id)
    {
        var result = await _mediator.Send(new DeactivateKpiMasterCommand(id, null));
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Deactivated successfully"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
    }

    [HttpPost("bulk-import")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<BulkImportResultDto>>> BulkImport(BulkImportKpiMasterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<BulkImportResultDto>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<BulkImportResultDto>.FailureResponse(result.Message, result.Errors));
    }
}
