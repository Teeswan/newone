using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.UseCases.PositionKpi.Commands;
using EPMS.Application.UseCases.PositionKpi.Queries;
using EPMS.Shared.Common;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using EPMS.Application.Interfaces;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/position-kpi")]
[Authorize]
public class PositionKpiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IExcelPdfService _excelPdfService;

    public PositionKpiController(IMediator mediator, IExcelPdfService excelPdfService)
    {
        _mediator = mediator;
        _excelPdfService = excelPdfService;
    }

    [HttpGet]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionKpiDto>>>> GetList([FromQuery] GetPositionKpiListQuery query)
     {
        var result = await _mediator.Send(query);
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<PositionKpiDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<PositionKpiDto>>.FailureResponse(result.Message, result.Errors));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<PositionKpiDto>>> GetById(int id)
    {
        var result = await _mediator.Send(new GetPositionKpiByIdQuery(id));
        return result.IsSuccess 
            ? Ok(ApiResponse<PositionKpiDto>.SuccessResponse(result.Value!))
            : NotFound(ApiResponse<PositionKpiDto>.FailureResponse(result.Message));
    }

    [HttpGet("by-position/{positionId}")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionKpiDto>>>> GetByPosition(int positionId)
    {
        var result = await _mediator.Send(new GetPositionKpiQuery(positionId));
        return result.IsSuccess 
            ? Ok(ApiResponse<IEnumerable<PositionKpiDto>>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<IEnumerable<PositionKpiDto>>.FailureResponse(result.Message));
    }

    [HttpPost]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreatePositionKpiCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, ApiResponse<int>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<int>.FailureResponse(result.Message, result.Errors));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdatePositionKpiCommand command)
    {
        if (id != command.PositionKpiId) return BadRequest(ApiResponse<object>.FailureResponse("ID mismatch"));
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Updated successfully"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message, result.Errors));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeletePositionKpiCommand(id, null));
        return result.IsSuccess 
            ? Ok(ApiResponse<object>.SuccessResponse(null!, "Deleted successfully"))
            : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
    }

    [HttpPost("bulk-import")]
    [HasPermission(Permissions.Kpis.Manage)]
    public async Task<ActionResult<ApiResponse<BulkImportResultDto>>> BulkImport(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(ApiResponse<BulkImportResultDto>.FailureResponse("No file uploaded"));

        using var stream = file.OpenReadStream();
        var kpis = await _excelPdfService.ImportPositionKpiFromExcelAsync(stream);
        
        var command = new BulkImportPositionKpiCommand(new List<PositionKpiImportDto>(kpis), null);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess 
            ? Ok(ApiResponse<BulkImportResultDto>.SuccessResponse(result.Value!))
            : BadRequest(ApiResponse<BulkImportResultDto>.FailureResponse(result.Message, result.Errors));
    }

    [HttpGet("import-template")]
    [HasPermission(Permissions.Kpis.View)]
    public async Task<IActionResult> DownloadTemplate()
    {
        var bytes = await _excelPdfService.ExportPositionKpiTemplateAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PositionKpiTemplate.xlsx");
    }
}
