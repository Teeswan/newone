using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record DeactivatePositionKpiCommand(int KpiId, int? EmployeeId) : IRequest<Result>;

public class DeactivatePositionKpiCommandHandler : IRequestHandler<DeactivatePositionKpiCommand, Result>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public DeactivatePositionKpiCommandHandler(
        IPositionKpiRepository repository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(DeactivatePositionKpiCommand request, CancellationToken cancellationToken)
    {
        var kpi = await _repository.GetByIdAsync(request.KpiId);
        if (kpi == null) return Result.Failure("KPI not found.");

        if (await _repository.IsKpiReferencedByActiveCycleAsync(request.KpiId))
        {
            return Result.Failure("Cannot deactivate KPI as it is currently referenced in an active appraisal cycle.");
        }

        kpi.Deactivate();
        await _repository.UpdateAsync(kpi);

        await _cacheService.RemoveAsync($"positionkpi:id:{kpi.KpiId}");
        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "SoftDelete", kpi.KpiId, $"Deactivated KPI: {kpi.KpiName}", request.EmployeeId);

        return Result.Success();
    }
}
