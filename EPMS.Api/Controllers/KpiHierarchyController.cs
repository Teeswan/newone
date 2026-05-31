using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KpiHierarchyController : ControllerBase
    {
        private readonly IKpiService _kpiService;
        private readonly IDepartmentKpiService _deptKpiService;
        private readonly ITeamKpiService _teamKpiService;

        public KpiHierarchyController(
            IKpiService kpiService,
            IDepartmentKpiService deptKpiService,
            ITeamKpiService teamKpiService
            )
        {
            _kpiService = kpiService;
            _deptKpiService = deptKpiService;
            _teamKpiService = teamKpiService;
        }

        // Master KPIs
        [HttpGet("master")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<IEnumerable<KpiDto>>> GetAllMaster() => Ok(await _kpiService.GetAllAsync());

        [HttpGet("master/{id}")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<KpiDto>> GetMasterById(int id)
        {
            var result = await _kpiService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("master")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<KpiDto>> CreateMaster(KpiRequest request)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdStr, out int id) ? id : null;
            
            var result = await _kpiService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetMasterById), new { id = result.KpiId }, result);
        }

        [HttpPut("master/{id}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<KpiDto>> UpdateMaster(int id, KpiRequest request)
        {
            var result = await _kpiService.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("master/{id}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<IActionResult> DeleteMaster(int id)
        {
            var result = await _kpiService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        // Department KPIs
        [HttpGet("department")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<IEnumerable<DepartmentKpiDto>>> GetAllDept() => Ok(await _deptKpiService.GetAllAsync());

        [HttpGet("department/by-dept/{deptId}/{cycleId}")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<IEnumerable<DepartmentKpiDto>>> GetDeptByDept(int deptId, int cycleId) => Ok(await _deptKpiService.GetByDepartmentIdAsync(deptId, cycleId));

        [HttpPost("department")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<DepartmentKpiDto>> CreateDept(DepartmentKpiRequest request)
        {
            var result = await _deptKpiService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("department/{id}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<DepartmentKpiDto>> UpdateDept(int id, DepartmentKpiRequest request)
        {
            var result = await _deptKpiService.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("department/calculate/{deptId}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<IEnumerable<DepartmentKpiDto>>> CalculateDepartment(int deptId)
        {
            try
            {
                var result = await _deptKpiService.CalculateForDepartmentAsync(deptId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Team KPIs
        [HttpGet("team")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<IEnumerable<TeamKpiDto>>> GetAllTeam() => Ok(await _teamKpiService.GetAllAsync());

        [HttpGet("team/by-team/{teamId}")]
        [HasPermission(Permissions.Kpis.View)]
        public async Task<ActionResult<IEnumerable<TeamKpiDto>>> GetTeamByTeam(int teamId) => Ok(await _teamKpiService.GetByTeamIdAsync(teamId));

        [HttpPost("team")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<TeamKpiDto>> CreateTeam(TeamKpiRequest request)
        {
            var result = await _teamKpiService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("team/{id}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<TeamKpiDto>> UpdateTeam(int id, TeamKpiRequest request)
        {
            var result = await _teamKpiService.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("team/calculate/{teamId}")]
        [HasPermission(Permissions.Kpis.Manage)]
        public async Task<ActionResult<IEnumerable<TeamKpiDto>>> CalculateTeam(int teamId)
        {
            try
            {
                var result = await _teamKpiService.CalculateForTeamAsync(teamId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
       
    }
}
