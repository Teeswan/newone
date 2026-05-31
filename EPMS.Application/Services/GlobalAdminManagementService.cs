using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class GlobalAdminManagementService : IGlobalAdminManagementService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionPermissionRepository _positionPermissionRepository;
    private readonly IDepartmentService _departmentService;
    private readonly IMapper _mapper;

    public GlobalAdminManagementService(
        IDepartmentRepository departmentRepository,
        ITeamRepository teamRepository,
        IEmployeeRepository employeeRepository,
        IPositionPermissionRepository positionPermissionRepository,
        IDepartmentService departmentService,
        IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _teamRepository = teamRepository;
        _employeeRepository = employeeRepository;
        _positionPermissionRepository = positionPermissionRepository;
        _departmentService = departmentService;
        _mapper = mapper;
    }

    public async Task<GlobalAdminOrganizationDto> GetFullOrganizationAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);

        var departments = await _departmentRepository.GetAllAsync();
        var teams = await _teamRepository.GetAllAsync();
        var employees = await _employeeRepository.GetAllAsync();

        var departmentDtos = _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
        var teamDtos = _mapper.Map<IReadOnlyList<TeamDto>>(teams);
        var employeeDtos = _mapper.Map<IReadOnlyList<EmployeeDto>>(employees)
            .DistinctBy(e => e.EmployeeId)
            .OrderBy(e => e.FullName)
            .ToList();

        return new GlobalAdminOrganizationDto
        {
            Departments = departmentDtos,
            Teams = teamDtos,
            Employees = employeeDtos,
            Summary = new GlobalAdminOrganizationSummary
            {
                DepartmentCount = departmentDtos.Count,
                TeamCount = teamDtos.Count,
                EmployeeCount = employeeDtos.Count,
                ActiveEmployeeCount = employeeDtos.Count(e => e.IsActive == true)
            }
        };
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        var departments = await _departmentRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
    }

    public async Task<IReadOnlyList<TeamDto>> GetAllTeamsAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        var teams = await _teamRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<TeamDto>>(teams);
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        var employees = await _employeeRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(employees)
            .DistinctBy(e => e.EmployeeId)
            .OrderBy(e => e.FullName)
            .ToList();
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(
        int currentPositionId,
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        return await _departmentService.CreateAsync(request);
    }

    public async Task<DepartmentDto?> UpdateDepartmentAsync(
        int currentPositionId,
        int departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        return await _departmentService.UpdateAsync(departmentId, request);
    }

    public async Task<bool> DeleteDepartmentAsync(
        int currentPositionId,
        int departmentId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGlobalAdminPermissionAsync(currentPositionId, cancellationToken);
        return await _departmentService.DeleteAsync(departmentId);
    }

    private async Task EnsureGlobalAdminPermissionAsync(int positionId, CancellationToken cancellationToken)
    {
        var permissions = await _positionPermissionRepository.GetPermissionsByPositionAsync(positionId);
        if (!permissions.Any(p => string.Equals(p.PermissionCode, Permissions.GlobalAdminManagement, StringComparison.Ordinal)))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }
    }
}
