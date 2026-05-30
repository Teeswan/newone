using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogsController(IAuditLogService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(Permissions.Security.Manage)] // Assuming viewing logs requires high-level permissions
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("entity/{entityName}")]
    [HasPermission(Permissions.Security.Manage)]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByEntity(string entityName)
    {
        var result = await _service.GetByEntityAsync(entityName);
        return Ok(result);
    }
}
