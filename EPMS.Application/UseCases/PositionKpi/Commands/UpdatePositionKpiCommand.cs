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
    public int PositionKpiId { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal TargetValue { get; set; }
    public int? PositionId { get; set; }
    public int? UpdatedByEmployeeId { get; set; }

    public class Validator : AbstractValidator<UpdatePositionKpiCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PositionKpiId).GreaterThan(0);
            RuleFor(x => x.WeightPercent).InclusiveBetween(0, 100);
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
        var positionKpi = await _repository.GetByIdAsync(request.PositionKpiId);
        if (positionKpi == null) return Result.Failure("KPI assignment not found.");

        if (!positionKpi.IsActive) return Result.Failure("Cannot update an inactive KPI assignment.");

        positionKpi.Update(
            request.WeightPercent,
            positionKpi.IsRequired,
            request.TargetValue);

        await _repository.UpdateAsync(positionKpi);

        await _cacheService.RemoveByPatternAsync("positionkpi:*");
        await _auditLogService.LogAsync("PositionKpi", "Update", positionKpi.PositionKpiId, $"Updated position KPI assignment", request.UpdatedByEmployeeId);

        return Result.Success();
    }
}
