using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record DeletePositionKpiCommand(int PositionKpiId, int? EmployeeId) : IRequest<Result>;

public class DeletePositionKpiCommandHandler : IRequestHandler<DeletePositionKpiCommand, Result>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public DeletePositionKpiCommandHandler(
        IPositionKpiRepository repository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(DeletePositionKpiCommand request, CancellationToken cancellationToken)
    {
        var positionKpi = await _repository.GetByIdAsync(request.PositionKpiId);
        if (positionKpi == null) return Result.Failure("KPI not found.");

        // Check if referenced by ANY cycle, not just active ones, to maintain data integrity
        if (await _repository.IsKpiReferencedByActiveCycleAsync(positionKpi.KpiId))
        {
            return Result.Failure("Cannot delete KPI as it is currently referenced in an appraisal cycle.");
        }

        await _repository.DeleteAsync(request.PositionKpiId);

        await _cacheService.RemoveAsync($"positionkpi:id:{positionKpi.PositionKpiId}");
        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "Delete", positionKpi.PositionKpiId, $"Deleted KPI: {positionKpi.Kpi.KpiName}", request.EmployeeId);

        return Result.Success();
    }
}
