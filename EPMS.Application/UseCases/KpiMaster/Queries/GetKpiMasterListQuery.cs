using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiMaster.Queries;

public record GetKpiMasterListQuery(int? PositionId, bool IsActive = true, string? Category = null) : IRequest<Result<IEnumerable<KpiMasterDto>>>;

public class GetKpiMasterListQueryHandler : IRequestHandler<GetKpiMasterListQuery, Result<IEnumerable<KpiMasterDto>>>
{
    private readonly IKpiQueryService _queryService;
    private readonly IKpiCacheService _cacheService;

    public GetKpiMasterListQueryHandler(IKpiQueryService queryService, IKpiCacheService cacheService)
    {
        _queryService = queryService;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<KpiMasterDto>>> Handle(GetKpiMasterListQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"kpimaster:list:pos:{request.PositionId ?? 0}:act:{request.IsActive}";
        var cached = await _cacheService.GetAsync<IEnumerable<KpiMasterDto>>(cacheKey);
        if (cached != null) return Result<IEnumerable<KpiMasterDto>>.Success(cached);

        var kpis = await _queryService.GetKpisByPositionAsync(request.PositionId, request.IsActive);

        await _cacheService.SetAsync(cacheKey, kpis, TimeSpan.FromMinutes(10));
        return Result<IEnumerable<KpiMasterDto>>.Success(kpis);
    }
}
