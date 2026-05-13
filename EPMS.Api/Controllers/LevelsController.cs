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
[AllowAnonymous]
public class LevelsController : ControllerBase
{
    private readonly ILevelService _service;

    public LevelsController(ILevelService service)
    {
        _service = service;
    }

    [HttpGet]
    //[HasPermission(Permissions.Levels.View)]
    public async Task<ActionResult<IEnumerable<LevelDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    //[HasPermission(Permissions.Levels.View)]
    public async Task<ActionResult<LevelDto>> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    //[HasPermission(Permissions.Levels.Manage)]
    public async Task<ActionResult<LevelDto>> Create(CreateLevelRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.LevelId }, result);
    }

    [HttpPut("{id}")]
    //[HasPermission(Permissions.Levels.Manage)]
    public async Task<ActionResult<LevelDto>> Update(string id, UpdateLevelRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    //[HasPermission(Permissions.Levels.Manage)]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
