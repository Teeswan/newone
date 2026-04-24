using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using FluentValidation;
using MediatR;

namespace EPMS.Application.UseCases.KpiMaster.Commands;

public record CreateKpiMasterCommand : IRequest<Result<int>>
{
    public string KpiName { get; init; } = null!;
    public string? Category { get; init; }
    public string? Unit { get; init; }
    public decimal WeightPercent { get; init; }
    public decimal? TargetValue { get; init; }
    public PriorityLevel PriorityLevel { get; init; }
    public KpiDirection Direction { get; init; }
    public int? PositionId { get; init; }
    public int? CreatedByUserId { get; init; }

    public class Validator : AbstractValidator<CreateKpiMasterCommand>
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

public class CreateKpiMasterCommandHandler : IRequestHandler<CreateKpiMasterCommand, Result<int>>
{
    private readonly IKpiMasterRepository _repository;
    private readonly IKpiCacheService _cacheService;
    private readonly IAuditLogService _auditLogService;

    public CreateKpiMasterCommandHandler(
        IKpiMasterRepository repository,
        IKpiCacheService cacheService,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditLogService = auditLogService;
    }

    public async Task<Result<int>> Handle(CreateKpiMasterCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsDuplicateAsync(request.KpiName, request.Category, request.PositionId))
        {
            return Result<int>.Failure("A KPI with the same name and category already exists for this position.");
        }

        var kpi = Domain.Entities.KpiMaster.Create(
            request.KpiName,
            request.Category,
            request.Unit,
            request.WeightPercent,
            request.TargetValue,
            request.PriorityLevel,
            request.Direction,
            request.PositionId,
            request.CreatedByUserId);

        await _repository.AddAsync(kpi);

        await _cacheService.RemoveByPatternAsync("kpimaster:list:*");
        await _auditLogService.LogAsync("KpiMaster", "Create", kpi.KpiId, $"Created KPI: {kpi.KpiName}", request.CreatedByUserId);

        return Result<int>.Success(kpi.KpiId);
    }
}
