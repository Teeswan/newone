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

    public GetPositionKpiQueryHandler(IKpiQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<IEnumerable<PositionKpiDto>>> Handle(GetPositionKpiQuery request, CancellationToken cancellationToken)
    {
        var kpis = await _queryService.GetKpiByPositionAsync(request.PositionId);
        return Result<IEnumerable<PositionKpiDto>>.Success(kpis);
    }
}
