using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _service;
    private readonly IBaseRepository<Permission, int> _permissionRepository;
    private readonly IPositionPermissionRepository _positionPermissionRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;

    public PermissionsController(
        IPermissionService service,
        IBaseRepository<Permission, int> permissionRepository,
        IPositionPermissionRepository positionPermissionRepository,
        IPositionRepository positionRepository,
        IEmployeeRepository employeeRepository,
        IUserRepository userRepository)
    {
        _service = service;
        _permissionRepository = permissionRepository;
        _positionPermissionRepository = positionPermissionRepository;
        _positionRepository = positionRepository;
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    //[HasPermission(Permissions.Security.ViewPermissions)]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("position/{positionId}")]
    //[HasPermission(Permissions.Security.ViewPermissions)]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetByPosition(int positionId)
    {
        var result = await _service.GetByPositionAsync(positionId);
        return Ok(result);
    }

    [HttpPost("assign")]
    //[HasPermission(Permissions.Security.Manage)]
    public async Task<IActionResult> AssignPermission(AssignPermissionRequest request)
    {
        var result = await _service.AssignPermissionAsync(request);
        if (!result) return BadRequest();
        return Ok();
    }

    [HttpPost("revoke")]
    //[HasPermission(Permissions.Security.Manage)]
    public async Task<IActionResult> RevokePermission(RevokePermissionRequest request)
    {
        var result = await _service.RevokePermissionAsync(request);
        if (!result) return BadRequest();
        return Ok();
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedAll()
    {
        // 1. Seed all permissions
        var permissionsToSeed = new List<(string Code, string Desc)>
        {
            ("Permissions.Employees.View", "View Employees"),
            ("Permissions.Employees.Manage", "Manage Employees"),
            ("Permissions.Departments.View", "View Departments"),
            ("Permissions.Departments.Manage", "Manage Departments"),
            ("Permissions.Teams.View", "View Teams"),
            ("Permissions.Teams.Manage", "Manage Teams"),
            ("Permissions.Levels.View", "View Levels"),
            ("Permissions.Levels.Manage", "Manage Levels"),
            ("Permissions.Positions.View", "View Positions"),
            ("Permissions.Positions.Manage", "Manage Positions"),
            ("Permissions.Security.ViewPermissions", "View Security Permissions"),
            ("Permissions.Security.Manage", "Manage Security Settings"),
            ("Permissions.AppraisalCycles.View", "View Appraisal Cycles"),
            ("Permissions.AppraisalCycles.Manage", "Manage Appraisal Cycles"),
            ("Permissions.AppraisalForms.View", "View Appraisal Forms"),
            ("Permissions.AppraisalForms.Manage", "Manage Appraisal Forms"),
            ("Permissions.AppraisalQuestions.View", "View Appraisal Questions"),
            ("Permissions.AppraisalQuestions.Manage", "Manage Appraisal Questions"),
            ("Permissions.AppraisalResponses.View", "View Appraisal Responses"),
            ("Permissions.AppraisalResponses.Manage", "Manage Appraisal Responses"),
            ("Permissions.PerformanceEvaluations.View", "View Performance Evaluations"),
            ("Permissions.PerformanceEvaluations.Manage", "Manage Performance Evaluations"),
            ("Permissions.PerformanceOutcomes.View", "View Performance Outcomes"),
            ("Permissions.PerformanceOutcomes.Manage", "Manage Performance Outcomes"),
            ("Permissions.Meetings.View", "View Meetings"),
            ("Permissions.Meetings.Manage", "Manage Meetings"),
            ("Permissions.Kpis.View", "View KPIs"),
            ("Permissions.Kpis.Manage", "Manage KPIs")
        };

        foreach (var (code, desc) in permissionsToSeed)
        {
            var existing = await _permissionRepository.GetAllAsync();
            if (!existing.Any(p => p.PermissionCode == code))
            {
                await _permissionRepository.CreateAsync(new Permission { PermissionCode = code, Description = desc });
            }
        }

        // 2. Seed Admin position
        Position? adminPosition = null;
        var allPositions = await _positionRepository.GetAllAsync();
        adminPosition = allPositions.FirstOrDefault(p => p.PositionTitle == "Admin");
        
        if (adminPosition == null)
        {
            adminPosition = new Position { PositionTitle = "Admin" };
            adminPosition = await _positionRepository.CreateAsync(adminPosition);
        }

        // 3. Assign all permissions to Admin
        var allPermissions = await _permissionRepository.GetAllAsync();
        foreach (var perm in allPermissions)
        {
            var existing = await _positionPermissionRepository.GetByPositionAndPermissionAsync(adminPosition.PositionId, perm.PermissionId);
            if (existing == null)
            {
                await _positionPermissionRepository.CreateAsync(new PositionPermission 
                { 
                    PositionId = adminPosition.PositionId, 
                    PermissionId = perm.PermissionId 
                });
            }
        }

        // 4. Seed Admin employee
        Employee? adminEmployee = null;
        var allEmployees = await _employeeRepository.GetAllAsync();
        adminEmployee = allEmployees.FirstOrDefault(e => e.EmployeeCode == "EMP001");
        
        if (adminEmployee == null)
        {
            adminEmployee = new Employee 
            { 
                EmployeeCode = "EMP001", 
                FullName = "System Admin",
                PositionId = adminPosition.PositionId,
                IsActive = true
            };
            adminEmployee = await _employeeRepository.CreateAsync(adminEmployee);
        }
        else if (adminEmployee.PositionId != adminPosition.PositionId)
        {
            adminEmployee.PositionId = adminPosition.PositionId;
            await _employeeRepository.UpdateAsync(adminEmployee);
        }

        // 5. Seed Admin user
        var allUsers = await _userRepository.GetAllAsync();
        var adminUser = allUsers.FirstOrDefault(u => u.Username == "admin");
        
        if (adminUser == null)
        {
            adminUser = new User 
            { 
                Username = "admin", 
                PasswordHash = "admin123",
                EmployeeId = adminEmployee.EmployeeId
            };
            await _userRepository.CreateAsync(adminUser);
        }

        return Ok(new 
        { 
            Message = "Seed completed successfully!",
            AdminPositionId = adminPosition.PositionId,
            AdminEmployeeId = adminEmployee.EmployeeId
        });
    }
}
