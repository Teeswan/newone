using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Queries;

public record GetPositionKpiQuery(int PositionId) : IRequest<Result<IEnumerable<PositionKpiDto>>>;

public class GetPositionKpiQueryHandler : IRequestHandler<GetPositionKpiQuery, Result<IEnumerable<PositionKpiDto>>>
{
    private readonly IKpiQueryService _queryService;
    private readonly IKpiCacheService _cacheService;

    public GetPositionKpiQueryHandler(IKpiQueryService queryService, IKpiCacheService cacheService)
    {
        _queryService = queryService;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<PositionKpiDto>>> Handle(GetPositionKpiQuery request, CancellationToken cancellationToken)
    {
        var isGlobal = request.PositionId <= 0;
        var cacheKey = $"positionkpi:by-position:{(isGlobal ? "global" : request.PositionId)}:v1";
        var cached = await _cacheService.GetAsync<IEnumerable<PositionKpiDto>>(cacheKey);
        if (cached != null) return Result<IEnumerable<PositionKpiDto>>.Success(cached);

        var kpis = await _queryService.GetKpiByPositionAsync(
            isGlobal ? null : request.PositionId, 
            isActive: true, 
            globalOnly: isGlobal);
        
        await _cacheService.SetAsync(cacheKey, kpis, TimeSpan.FromMinutes(10));
        
        return Result<IEnumerable<PositionKpiDto>>.Success(kpis);
    }
}
