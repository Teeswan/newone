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

public record GetPositionKpiListQuery(
    int? PositionId = null, 
    bool IsActive = true, 
    string? Category = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null
) : IRequest<Result<PaginatedResult<PositionKpiDto>>>;

public class GetPositionKpiListQueryHandler : IRequestHandler<GetPositionKpiListQuery, Result<PaginatedResult<PositionKpiDto>>>
{
    private readonly IKpiQueryService _queryService;
    private readonly IKpiCacheService _cacheService;

    public GetPositionKpiListQueryHandler(IKpiQueryService queryService, IKpiCacheService cacheService)
    {
        _queryService = queryService;
        _cacheService = cacheService;
    }

    public async Task<Result<PaginatedResult<PositionKpiDto>>> Handle(GetPositionKpiListQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"positionkpi:paged:pos:{request.PositionId ?? 0}:act:{request.IsActive}:p:{request.PageNumber}:s:{request.PageSize}:q:{request.SearchTerm ?? "none"}:v1";
        var cached = await _cacheService.GetAsync<PaginatedResult<PositionKpiDto>>(cacheKey);
        if (cached != null) return Result<PaginatedResult<PositionKpiDto>>.Success(cached);

        var result = await _queryService.GetPagedPositionKpiAsync(
            request.PositionId, 
            request.IsActive, 
            request.PageNumber, 
            request.PageSize, 
            request.SearchTerm);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        return Result<PaginatedResult<PositionKpiDto>>.Success(result);
    }
}
