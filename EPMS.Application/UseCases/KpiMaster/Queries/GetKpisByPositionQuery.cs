using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiMaster.Queries;

public record GetKpisByPositionQuery(int PositionId) : IRequest<Result<IEnumerable<KpiMasterDto>>>;

public class GetKpisByPositionQueryHandler : IRequestHandler<GetKpisByPositionQuery, Result<IEnumerable<KpiMasterDto>>>
{
    private readonly IKpiQueryService _queryService;

    public GetKpisByPositionQueryHandler(IKpiQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<IEnumerable<KpiMasterDto>>> Handle(GetKpisByPositionQuery request, CancellationToken cancellationToken)
    {
        var kpis = await _queryService.GetKpisByPositionAsync(request.PositionId);
        return Result<IEnumerable<KpiMasterDto>>.Success(kpis);
    }
}
