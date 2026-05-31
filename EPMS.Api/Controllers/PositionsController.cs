using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Common;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PositionsController : ControllerBase
{
    private readonly IPositionService _service;

    public PositionsController(IPositionService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PositionDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<ApiResponse<PositionDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<PositionDto>.FailureResponse("Position not found."));
        return Ok(ApiResponse<PositionDto>.SuccessResponse(result));
    }

    [HttpGet("level/{levelId}")]
    [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionDto>>>> GetByLevel(string levelId)
    {
        var result = await _service.GetByLevelAsync(levelId);
        return Ok(ApiResponse<IEnumerable<PositionDto>>.SuccessResponse(result));
    }

    [HttpPost]
    [HasPermission(Permissions.Positions.Manage)]                
    public async Task<ActionResult<ApiResponse<PositionDto>>> Create(CreatePositionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.PositionId }, ApiResponse<PositionDto>.SuccessResponse(result, "Position created successfully."));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Positions.Manage)]                
    public async Task<ActionResult<ApiResponse<PositionDto>>> Update(int id, UpdatePositionRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<PositionDto>.FailureResponse("Position not found."));
        return Ok(ApiResponse<PositionDto>.SuccessResponse(result, "Position updated successfully."));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Positions.Manage)]                
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<bool>.FailureResponse("Position not found."));
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Position deleted successfully."));
    }
}
