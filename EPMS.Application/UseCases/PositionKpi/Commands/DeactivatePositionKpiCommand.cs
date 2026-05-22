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
    private readonly IKpiRepository _kpiRepository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public DeactivatePositionKpiCommandHandler(
        IPositionKpiRepository repository,
        IKpiRepository kpiRepository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _kpiRepository = kpiRepository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(DeactivatePositionKpiCommand request, CancellationToken cancellationToken)
    {
        var positionKpi = await _repository.GetByIdAsync(request.KpiId);
        if (positionKpi == null) return Result.Failure("KPI not found.");

        if (await _repository.IsKpiReferencedByActiveCycleAsync(request.KpiId))
        {
            return Result.Failure("Cannot deactivate KPI as it is currently referenced in an active appraisal cycle.");
        }

        positionKpi.Kpi.Deactivate();
        await _kpiRepository.UpdateAsync(positionKpi.Kpi);

        await _cacheService.RemoveAsync($"positionkpi:id:{positionKpi.PositionKpiId}");
        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "SoftDelete", positionKpi.PositionKpiId, $"Deactivated KPI: {positionKpi.Kpi.KpiName}", request.EmployeeId);

        return Result.Success();
    }
}
