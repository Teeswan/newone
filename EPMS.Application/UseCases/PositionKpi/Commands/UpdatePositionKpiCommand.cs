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
    public int KpiId { get; set; }
    public string KpiName { get; set; } = null!;
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal? TargetValue { get; set; }
    public PriorityLevel PriorityLevel { get; set; }
    public KpiDirection Direction { get; set; }
    public int? PositionId { get; set; }
    public int? UpdatedByEmployeeId { get; set; }

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
    private readonly IKpiRepository _kpiRepository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public UpdatePositionKpiCommandHandler(
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

    public async Task<Result> Handle(UpdatePositionKpiCommand request, CancellationToken cancellationToken)
    {
        var positionKpi = await _repository.GetByIdAsync(request.KpiId);
        if (positionKpi == null) return Result.Failure("KPI not found.");

        if (!positionKpi.Kpi.IsActive) return Result.Failure("Cannot update an inactive KPI.");

        if (await _repository.IsKpiReferencedByActiveCycleAsync(request.KpiId))
        {
            return Result.Failure("Cannot update KPI as it is currently referenced in an active appraisal cycle.");
        }

        positionKpi.Kpi.Update(
            request.KpiName,
            request.Category,
            request.Unit,
            request.WeightPercent,
            request.TargetValue,
            request.PriorityLevel,
            request.Direction);

        positionKpi.Update(
            request.WeightPercent,
            positionKpi.IsRequired);

        await _kpiRepository.UpdateAsync(positionKpi.Kpi);
        await _repository.UpdateAsync(positionKpi);

        await _cacheService.RemoveAsync($"positionkpi:id:{positionKpi.PositionKpiId}");
        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "Update", positionKpi.PositionKpiId, $"Updated KPI: {request.KpiName}", request.UpdatedByEmployeeId);

        return Result.Success();
    }
}
