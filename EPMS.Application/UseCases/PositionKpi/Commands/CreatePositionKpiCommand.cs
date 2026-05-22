using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using FluentValidation;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record CreatePositionKpiCommand : IRequest<Result<int>>
{
    public string KpiName { get; init; } = null!;
    public string? Category { get; init; }
    public string? Unit { get; init; }
    public decimal WeightPercent { get; init; }
    public decimal? TargetValue { get; init; }
    public PriorityLevel PriorityLevel { get; init; }
    public KpiDirection Direction { get; init; }
    public int? PositionId { get; init; }
    public int? CreatedByEmployeeId { get; init; }

    public class Validator : AbstractValidator<CreatePositionKpiCommand>
    {
        public Validator()
        {
            RuleFor(x => x.KpiName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.WeightPercent).InclusiveBetween(0, 100);
            RuleFor(x => x.PriorityLevel).IsInEnum();
        }
    }
}

public class CreatePositionKpiCommandHandler : IRequestHandler<CreatePositionKpiCommand, Result<int>>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IKpiRepository _kpiRepository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public CreatePositionKpiCommandHandler(
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

    public async Task<Result<int>> Handle(CreatePositionKpiCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsDuplicateAsync(request.KpiName, request.Category, request.PositionId))
        {
            return Result<int>.Failure("A KPI with the same name and category already exists for this position.");
        }

        var kpiMaster = Domain.Entities.Kpi.Create(
            request.KpiName,
            request.Category,
            request.Unit,
            request.WeightPercent,
            request.TargetValue,
            request.PriorityLevel,
            request.Direction,
            request.CreatedByEmployeeId);

        await _kpiRepository.CreateAsync(kpiMaster);

        var positionKpi = Domain.Entities.PositionKpi.Create(
            request.PositionId,
            kpiMaster.KpiId,
            request.WeightPercent,
            true); // Defaulting to IsRequired = true

        await _repository.AddAsync(positionKpi);

        await _cacheService.RemoveByPatternAsync("positionkpi:list:*");
        await _auditLogService.LogAsync("PositionKpi", "Create", positionKpi.PositionKpiId, $"Created KPI: {request.KpiName}", request.CreatedByEmployeeId);

        return Result<int>.Success(positionKpi.PositionKpiId);
    }
}
