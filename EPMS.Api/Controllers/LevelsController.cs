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

public class LevelsController : ControllerBase
{
    private readonly ILevelService _service;

    public LevelsController(ILevelService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(Permissions.Levels.View)]                
    public async Task<ActionResult<ApiResponse<IEnumerable<LevelDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<LevelDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Levels.View)]                
    public async Task<ActionResult<ApiResponse<LevelDto>>> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<LevelDto>.FailureResponse("Level not found."));
        return Ok(ApiResponse<LevelDto>.SuccessResponse(result));
    }

    [HttpPost]
    [HasPermission(Permissions.Levels.Manage)]                
    public async Task<ActionResult<ApiResponse<LevelDto>>> Create(CreateLevelRequest request)
    {
        var result = await _service.CreateAsync(request);
        var response = ApiResponse<LevelDto>.SuccessResponse(result, "Level created successfully.");
        return CreatedAtAction(nameof(GetById), new { id = result.LevelId }, response);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Levels.Manage)]                
    public async Task<ActionResult<ApiResponse<LevelDto>>> Update(string id, UpdateLevelRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<LevelDto>.FailureResponse("Level not found."));
        return Ok(ApiResponse<LevelDto>.SuccessResponse(result, "Level updated successfully."));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Levels.Manage)]                
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<bool>.FailureResponse("Level not found."));
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Level deleted successfully."));
    }
}
