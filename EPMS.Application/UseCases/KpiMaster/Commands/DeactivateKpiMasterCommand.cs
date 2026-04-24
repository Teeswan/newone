using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiMaster.Commands;

public record DeactivateKpiMasterCommand(int KpiId, int? UserId) : IRequest<Result>;

public class DeactivateKpiMasterCommandHandler : IRequestHandler<DeactivateKpiMasterCommand, Result>
{
    private readonly IKpiMasterRepository _repository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public DeactivateKpiMasterCommandHandler(
        IKpiMasterRepository repository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(DeactivateKpiMasterCommand request, CancellationToken cancellationToken)
    {
        var kpi = await _repository.GetByIdAsync(request.KpiId);
        if (kpi == null) return Result.Failure("KPI not found.");

        if (await _repository.IsKpiReferencedByActiveCycleAsync(request.KpiId))
        {
            return Result.Failure("Cannot deactivate KPI as it is currently referenced in an active appraisal cycle.");
        }

        kpi.Deactivate();
        await _repository.UpdateAsync(kpi);

        await _cacheService.RemoveAsync($"kpimaster:id:{kpi.KpiId}");
        await _cacheService.RemoveByPatternAsync("kpimaster:list:*");
        await _auditLogService.LogAsync("KpiMaster", "SoftDelete", kpi.KpiId, $"Deactivated KPI: {kpi.KpiName}", request.UserId);

        return Result.Success();
    }
}
