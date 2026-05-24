using System;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Queries;

public record GetPositionKpiByIdQuery(int Id) : IRequest<Result<PositionKpiDto>>;

public class GetPositionKpiByIdQueryHandler : IRequestHandler<GetPositionKpiByIdQuery, Result<PositionKpiDto>>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IKpiCacheService _cacheService;

    public GetPositionKpiByIdQueryHandler(IPositionKpiRepository repository, IKpiCacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<PositionKpiDto>> Handle(GetPositionKpiByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"positionkpi:id:{request.Id}";
        var cached = await _cacheService.GetAsync<PositionKpiDto>(cacheKey);
        if (cached != null) return Result<PositionKpiDto>.Success(cached);

        var kpi = await _repository.GetByIdAsync(request.Id);
        if (kpi == null) return Result<PositionKpiDto>.Failure("KPI not found.");

        var dto = new PositionKpiDto
        {
            PositionKpiId = kpi.PositionKpiId,
            KpiId = kpi.KpiId,
            KpiName = kpi.Kpi.KpiName,
            Category = kpi.Kpi.Category,
            Unit = kpi.Kpi.Unit,
            WeightPercent = kpi.DefaultWeightPercent,
            TargetValue = kpi.Kpi.TargetValue,
            PriorityLevel = kpi.Kpi.PriorityLevel,
            Direction = kpi.Kpi.Direction,
            PositionId = kpi.PositionId,
            PositionName = kpi.Position?.PositionTitle,
            IsRequired = kpi.IsRequired,
            IsActive = kpi.Kpi.IsActive
        };

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));
        return Result<PositionKpiDto>.Success(dto);
    }
}
