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

public class PositionsController : ControllerBase
{
    private readonly IPositionService _service;

    public PositionsController(IPositionService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<IEnumerable<PositionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<PositionDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("level/{levelId}")]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.View)]                
    public async Task<ActionResult<IEnumerable<PositionDto>>> GetByLevel(string levelId)
    {
        var result = await _service.GetByLevelAsync(levelId);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.Manage)]                
    public async Task<ActionResult<PositionDto>> Create(CreatePositionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.PositionId }, result);
    }

    [HttpPut("{id}")]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.Manage)]                
    public async Task<ActionResult<PositionDto>> Update(int id, UpdatePositionRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [AllowAnonymous]
    // [HasPermission(Permissions.Positions.Manage)]                
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
