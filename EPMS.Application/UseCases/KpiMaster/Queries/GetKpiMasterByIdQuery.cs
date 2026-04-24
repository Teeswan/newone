using System;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiMaster.Queries;

public record GetKpiMasterByIdQuery(int Id) : IRequest<Result<KpiMasterDto>>;

public class GetKpiMasterByIdQueryHandler : IRequestHandler<GetKpiMasterByIdQuery, Result<KpiMasterDto>>
{
    private readonly IKpiMasterRepository _repository;
    private readonly IKpiCacheService _cacheService;

    public GetKpiMasterByIdQueryHandler(IKpiMasterRepository repository, IKpiCacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<KpiMasterDto>> Handle(GetKpiMasterByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"kpimaster:id:{request.Id}";
        var cached = await _cacheService.GetAsync<KpiMasterDto>(cacheKey);
        if (cached != null) return Result<KpiMasterDto>.Success(cached);

        var kpi = await _repository.GetByIdAsync(request.Id);
        if (kpi == null) return Result<KpiMasterDto>.Failure("KPI not found.");

        var dto = new KpiMasterDto
        {
            KpiId = kpi.KpiId,
            KpiName = kpi.KpiName,
            Category = kpi.Category,
            Unit = kpi.Unit,
            WeightPercent = kpi.WeightPercent,
            TargetValue = kpi.TargetValue,
            PriorityLevel = kpi.PriorityLevel,
            Direction = kpi.Direction,
            PositionId = kpi.PositionId,
            IsActive = kpi.IsActive
        };

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));
        return Result<KpiMasterDto>.Success(dto);
    }
}
