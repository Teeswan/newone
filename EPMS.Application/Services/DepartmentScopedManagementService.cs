using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class DepartmentScopedManagementService : IDepartmentScopedManagementService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionPermissionRepository _positionPermissionRepository;
    private readonly ITeamService _teamService;
    private readonly IMapper _mapper;

    public DepartmentScopedManagementService(
        ITeamRepository teamRepository,
        IEmployeeRepository employeeRepository,
        IPositionPermissionRepository positionPermissionRepository,
        ITeamService teamService,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _employeeRepository = employeeRepository;
        _positionPermissionRepository = positionPermissionRepository;
        _teamService = teamService;
        _mapper = mapper;
    }

    public async Task<DepartmentScopedAccessDto> GetScopedAccessAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);

        var teams = await _teamRepository.GetDepartmentTeamsAsync(scope.DepartmentId, cancellationToken);
        var employees = await _employeeRepository.GetDepartmentEmployeesAsync(scope.DepartmentId, cancellationToken);

        return new DepartmentScopedAccessDto
        {
            DepartmentId = scope.DepartmentId,
            Teams = _mapper.Map<IReadOnlyList<TeamDto>>(teams),
            Employees = _mapper.Map<IReadOnlyList<EmployeeDto>>(employees)
        };
    }

    public async Task<IReadOnlyList<TeamDto>> GetManageableTeamsAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);
        var teams = await _teamRepository.GetDepartmentTeamsAsync(scope.DepartmentId, cancellationToken);
        return _mapper.Map<IReadOnlyList<TeamDto>>(teams);
    }

    public async Task<TeamDto?> GetManageableTeamByIdAsync(
        int currentEmployeeId,
        int currentPositionId,
        int teamId,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);
        var team = await _teamRepository.GetByIdInDepartmentAsync(teamId, scope.DepartmentId, cancellationToken);
        return team == null ? null : _mapper.Map<TeamDto>(team);
    }

    public async Task<TeamDto> UpdateManageableTeamAsync(
        int currentEmployeeId,
        int currentPositionId,
        int teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);
        await EnsureTeamInDepartmentAsync(teamId, scope.DepartmentId, cancellationToken);

        request.DepartmentId = scope.DepartmentId;

        return await _teamService.UpdateAsync(teamId, request)
            ?? throw new KeyNotFoundException($"Team {teamId} was not found.");
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetViewableEmployeesAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);
        var employees = await _employeeRepository.GetDepartmentEmployeesAsync(scope.DepartmentId, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDetailDto?> GetViewableEmployeeByIdAsync(
        int currentEmployeeId,
        int currentPositionId,
        int targetEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAndResolveScopeAsync(currentEmployeeId, currentPositionId, cancellationToken);
        var employee = await _employeeRepository.GetByIdInDepartmentTeamsReadOnlyAsync(
            targetEmployeeId,
            scope.DepartmentId,
            cancellationToken);

        return employee == null ? null : _mapper.Map<EmployeeDetailDto>(employee);
    }

    /// <summary>
    /// Single RBAC gate + department resolution used by all department-scoped operations.
    /// </summary>
    private async Task<DepartmentScope> AuthorizeAndResolveScopeAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken)
    {
        await EnsureDepartmentScopedManagementPermissionAsync(currentPositionId, cancellationToken);
        var departmentId = await ResolveCurrentUserDepartmentIdAsync(currentEmployeeId, cancellationToken);
        return new DepartmentScope(departmentId);
    }

    private async Task EnsureDepartmentScopedManagementPermissionAsync(int positionId, CancellationToken cancellationToken)
    {
        var permissions = await _positionPermissionRepository.GetPermissionsByPositionAsync(positionId);
        if (!permissions.Any(p => string.Equals(p.PermissionCode, Permissions.DepartmentScopedManagement, StringComparison.Ordinal)))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }
    }

    private async Task<int> ResolveCurrentUserDepartmentIdAsync(int currentEmployeeId, CancellationToken cancellationToken)
    {
        var currentUser = await _employeeRepository.GetByIdNoTrackingAsync(currentEmployeeId);
        if (currentUser == null || !currentUser.DepartmentId.HasValue)
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        return currentUser.DepartmentId.Value;
    }

    private async Task EnsureTeamInDepartmentAsync(int teamId, int departmentId, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdInDepartmentAsync(teamId, departmentId, cancellationToken);
        if (team == null)
        {
            throw new UnauthorizedAccessException("Access Denied");
        }
    }

    private readonly record struct DepartmentScope(int DepartmentId);
}
