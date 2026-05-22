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
        private readonly IEmployeeKpiService _empKpiService;

        public KpiHierarchyController(
            IKpiService kpiService,
            IDepartmentKpiService deptKpiService,
            ITeamKpiService teamKpiService,
            IEmployeeKpiService empKpiService)
        {
            _kpiService = kpiService;
            _deptKpiService = deptKpiService;
            _teamKpiService = teamKpiService;
            _empKpiService = empKpiService;
        }

        // Master KPIs
        [HttpGet("master")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<KpiDto>>> GetAllMaster() => Ok(await _kpiService.GetAllAsync());

        [HttpGet("master/{id}")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<KpiDto>> GetMasterById(int id)
        {
            var result = await _kpiService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("master")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<ActionResult<KpiDto>> CreateMaster(KpiRequest request)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdStr, out int id) ? id : null;
            
            var result = await _kpiService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetMasterById), new { id = result.KpiId }, result);
        }

        [HttpPut("master/{id}")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<ActionResult<KpiDto>> UpdateMaster(int id, KpiRequest request)
        {
            var result = await _kpiService.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("master/{id}")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<IActionResult> DeleteMaster(int id)
        {
            var result = await _kpiService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        // Department KPIs
        [HttpGet("department")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<DepartmentKpiDto>>> GetAllDept() => Ok(await _deptKpiService.GetAllAsync());

        [HttpGet("department/by-dept/{deptId}/{cycleId}")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<DepartmentKpiDto>>> GetDeptByDept(int deptId, int cycleId) => Ok(await _deptKpiService.GetByDepartmentIdAsync(deptId, cycleId));

        [HttpPost("department")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<ActionResult<DepartmentKpiDto>> CreateDept(DepartmentKpiRequest request)
        {
            var result = await _deptKpiService.CreateAsync(request);
            return Ok(result);
        }

        // Team KPIs
        [HttpGet("team")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<TeamKpiDto>>> GetAllTeam() => Ok(await _teamKpiService.GetAllAsync());

        [HttpGet("team/by-team/{teamId}")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<TeamKpiDto>>> GetTeamByTeam(int teamId) => Ok(await _teamKpiService.GetByTeamIdAsync(teamId));

        [HttpPost("team")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<ActionResult<TeamKpiDto>> CreateTeam(TeamKpiRequest request)
        {
            var result = await _teamKpiService.CreateAsync(request);
            return Ok(result);
        }

        // Employee KPIs
        [HttpGet("employee")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<EmployeeKpiDto>>> GetAllEmp() => Ok(await _empKpiService.GetAllAsync());

        [HttpGet("employee/by-emp/{empId}")]
        [HasPermission(Permissions.KpiHierarchy.View)]
        public async Task<ActionResult<IEnumerable<EmployeeKpiDto>>> GetEmpByEmp(int empId) => Ok(await _empKpiService.GetByEmployeeIdAsync(empId));

        [HttpPost("employee")]
        [HasPermission(Permissions.KpiHierarchy.Manage)]
        public async Task<ActionResult<EmployeeKpiDto>> CreateEmp(EmployeeKpiRequest request)
        {
            var result = await _empKpiService.CreateAsync(request);
            return Ok(result);
        }
    }
}
