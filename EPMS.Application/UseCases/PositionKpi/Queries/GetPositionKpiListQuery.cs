using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Queries;

public record GetPositionKpiListQuery(int? PositionId, bool IsActive = true, string? Category = null) : IRequest<Result<IEnumerable<PositionKpiDto>>>;

public class GetPositionKpiListQueryHandler : IRequestHandler<GetPositionKpiListQuery, Result<IEnumerable<PositionKpiDto>>>
{
    private readonly IKpiQueryService _queryService;
    private readonly IKpiCacheService _cacheService;

    public GetPositionKpiListQueryHandler(IKpiQueryService queryService, IKpiCacheService cacheService)
    {
        _queryService = queryService;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<PositionKpiDto>>> Handle(GetPositionKpiListQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"positionkpi:list:pos:{request.PositionId ?? 0}:act:{request.IsActive}:cat:{request.Category ?? "all"}:v5";
        var cached = await _cacheService.GetAsync<IEnumerable<PositionKpiDto>>(cacheKey);
        if (cached != null) return Result<IEnumerable<PositionKpiDto>>.Success(cached);

        var kpis = await _queryService.GetKpiByPositionAsync(request.PositionId, request.IsActive);
        
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            kpis = kpis.Where(k => k.Category != null && 
                                k.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        await _cacheService.SetAsync(cacheKey, kpis, TimeSpan.FromMinutes(10));
        return Result<IEnumerable<PositionKpiDto>>.Success(kpis);
    }
}
