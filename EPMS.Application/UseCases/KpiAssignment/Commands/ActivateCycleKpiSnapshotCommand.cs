using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record ActivateCycleKpiSnapshotCommand(int CycleId, int? UserId) : IRequest<Result>;

public class ActivateCycleKpiSnapshotCommandHandler : IRequestHandler<ActivateCycleKpiSnapshotCommand, Result>
{
    private readonly IKpiMasterRepository _kpiRepository;
    private readonly IKpiAssignmentRepository _assignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuditLogService _auditLogService;

    public ActivateCycleKpiSnapshotCommandHandler(
        IKpiMasterRepository kpiRepository,
        IKpiAssignmentRepository assignmentRepository,
        IEmployeeRepository employeeRepository,
        IAuditLogService auditLogService)
    {
        _kpiRepository = kpiRepository;
        _assignmentRepository = assignmentRepository;
        _employeeRepository = employeeRepository;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(ActivateCycleKpiSnapshotCommand request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(); // Simplified
        var globalKpis = await _kpiRepository.GetGlobalKpisAsync();

        foreach (var employee in employees)
        {
            var positionKpis = await _kpiRepository.GetListByPositionAsync(employee.PositionId);
            var allKpis = globalKpis.Concat(positionKpis);

            var assignments = allKpis.Select(k => EmployeeKpiAssignment.CreateSnapshot(
                employee.EmployeeId,
                request.CycleId,
                k));

            await _assignmentRepository.AddRangeAsync(assignments);
        }

        await _auditLogService.LogAsync("EmployeeKpiAssignment", "SnapshotCreated", null, $"Created snapshots for cycle {request.CycleId}", request.UserId);

        return Result.Success();
    }
}
