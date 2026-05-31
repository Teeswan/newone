using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class TeamScopedEmployeeService : ITeamScopedEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionPermissionRepository _positionPermissionRepository;
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public TeamScopedEmployeeService(
        IEmployeeRepository employeeRepository,
        IPositionPermissionRepository positionPermissionRepository,
        IEmployeeService employeeService,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _positionPermissionRepository = positionPermissionRepository;
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetManageableEmployeesAsync(
        int currentEmployeeId,
        int currentPositionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTeamEmployeeManagementPermissionAsync(currentPositionId, cancellationToken);

        var employees = await _employeeRepository.GetEmployeesSharingTeamsWithAsync(currentEmployeeId, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDetailDto?> GetManageableEmployeeByIdAsync(
        int currentEmployeeId,
        int currentPositionId,
        int targetEmployeeId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTeamEmployeeManagementPermissionAsync(currentPositionId, cancellationToken);
        await EnsureTargetIsInScopeAsync(currentEmployeeId, targetEmployeeId, cancellationToken);

        return await _employeeService.GetByIdAsync(targetEmployeeId);
    }

    public async Task<EmployeeDto> UpdateManageableEmployeeAsync(
        int currentEmployeeId,
        int currentPositionId,
        int targetEmployeeId,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureTeamEmployeeManagementPermissionAsync(currentPositionId, cancellationToken);
        await EnsureTargetIsInScopeAsync(currentEmployeeId, targetEmployeeId, cancellationToken);

        var updated = await _employeeService.UpdateAsync(targetEmployeeId, request)
            ?? throw new KeyNotFoundException($"Employee {targetEmployeeId} was not found.");

        return updated;
    }

    public async Task<bool> DeleteManageableEmployeeAsync(
        int currentEmployeeId,
        int currentPositionId,
        int targetEmployeeId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTeamEmployeeManagementPermissionAsync(currentPositionId, cancellationToken);
        await EnsureTargetIsInScopeAsync(currentEmployeeId, targetEmployeeId, cancellationToken);

        return await _employeeService.DeleteAsync(targetEmployeeId);
    }

    /// <summary>
    /// RBAC: requires TeamEmployeeManagement on the caller's position.
    /// </summary>
    private async Task EnsureTeamEmployeeManagementPermissionAsync(int positionId, CancellationToken cancellationToken)
    {
        var permissions = await _positionPermissionRepository.GetPermissionsByPositionAsync(positionId);
        var hasPermission = permissions.Any(p =>
            string.Equals(p.PermissionCode, Permissions.Employees.TeamEmployeeManagement, StringComparison.Ordinal));

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Access Denied");
        }
    }

    /// <summary>
    /// Scope enforcement: target must share at least one team with the current user (M:N via TeamsNavigation).
    /// </summary>
    private async Task EnsureTargetIsInScopeAsync(
        int currentEmployeeId,
        int targetEmployeeId,
        CancellationToken cancellationToken)
    {
        if (currentEmployeeId == targetEmployeeId)
        {
            return;
        }

        var inScope = await _employeeRepository.SharesAnyTeamWithAsync(
            currentEmployeeId,
            targetEmployeeId,
            cancellationToken);

        if (!inScope)
        {
            throw new UnauthorizedAccessException("Access Denied");
        }
    }
}
