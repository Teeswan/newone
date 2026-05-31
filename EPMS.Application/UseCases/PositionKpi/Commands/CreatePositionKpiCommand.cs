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
    public int KpiId { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal TargetValue { get; set; }
    public int? PositionId { get; set; }
    public int? CreatedByEmployeeId { get; set; }

    public class Validator : AbstractValidator<CreatePositionKpiCommand>
    {
        public Validator()
        {
            RuleFor(x => x.KpiId).GreaterThan(0);
            RuleFor(x => x.WeightPercent).InclusiveBetween(0, 100);
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
        var existingKpi = await _kpiRepository.GetByIdAsync(request.KpiId);
        if (existingKpi == null) return Result<int>.Failure("Master KPI not found.");

        var positionKpi = Domain.Entities.PositionKpi.Create(
            request.PositionId,
            request.KpiId,
            request.WeightPercent,
            true, // Defaulting to IsRequired = true
            request.TargetValue);

        await _repository.AddAsync(positionKpi);

        await _cacheService.RemoveByPatternAsync("positionkpi:*");
        await _auditLogService.LogAsync("PositionKpi", "Create", positionKpi.PositionKpiId, $"Assigned KPI '{existingKpi.KpiName}' to position", request.CreatedByEmployeeId);

        return Result<int>.Success(positionKpi.PositionKpiId);
    }
}