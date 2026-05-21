using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using FluentValidation;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record UpdatePositionKpiCommand : IRequest<Result>
{
    public int KpiId { get; init; }
    public string KpiName { get; init; } = null!;
    public string? Category { get; init; }
    public string? Unit { get; init; }
    public decimal WeightPercent { get; init; }
    public decimal? TargetValue { get; init; }
    public PriorityLevel PriorityLevel { get; init; }
    public KpiDirection Direction { get; init; }
    public int? PositionId { get; init; }
    public int? UpdatedByEmployeeId { get; init; }

    public class Validator : AbstractValidator<UpdatePositionKpiCommand>
    {
        public Validator()
        {
            RuleFor(x => x.KpiId).GreaterThan(0);
            RuleFor(x => x.KpiName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.WeightPercent).InclusiveBetween(0, 100);
            RuleFor(x => x.PriorityLevel).IsInEnum();
        }
    }
}

public class UpdatePositionKpiCommandHandler : IRequestHandler<UpdatePositionKpiCommand, Result>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public UpdatePositionKpiCommandHandler(
        IPositionKpiRepository repository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(UpdatePositionKpiCommand request, CancellationToken cancellationToken)
    {
        var kpi = await _repository.GetByIdAsync(request.KpiId);
        if (kpi == null) return Result.Failure("KPI not found.");

        if (!kpi.IsActive) return Result.Failure("Cannot update an inactive KPI.");

        if (await _repository.IsKpiReferencedByActiveCycleAsync(request.KpiId))
        {
            return Result.Failure("Cannot update KPI as it is currently referenced in an active appraisal cycle.");
        }

        kpi.Update(
            request.KpiName,
            request.Category,
            request.Unit,
            request.WeightPercent,
            request.TargetValue,
            request.PriorityLevel,
            request.Direction,
            request.PositionId);

        await _repository.UpdateAsync(kpi);

        await _cacheService.RemoveAsync($"positionkpi:id:{kpi.KpiId}");
        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "Update", kpi.KpiId, $"Updated KPI: {kpi.KpiName}", request.UpdatedByEmployeeId);

        return Result.Success();
    }
}
