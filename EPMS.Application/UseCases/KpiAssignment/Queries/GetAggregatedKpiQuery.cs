using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Enums;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Queries;

public record GetAggregatedKpiQuery(int CycleId, AggregationType Type) : IRequest<Result<IEnumerable<AggregatedKpiDto>>>;

public class GetAggregatedKpiQueryHandler : IRequestHandler<GetAggregatedKpiQuery, Result<IEnumerable<AggregatedKpiDto>>>
{
    private readonly IKpiQueryService _queryService;

    public GetAggregatedKpiQueryHandler(IKpiQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<IEnumerable<AggregatedKpiDto>>> Handle(GetAggregatedKpiQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AggregatedKpiDto> results;

        switch (request.Type)
        {
            case AggregationType.Department:
                results = await _queryService.GetDepartmentKpiSummaryAsync(request.CycleId);
                break;
            case AggregationType.Team:
                results = await _queryService.GetTeamKpiSummaryAsync(request.CycleId);
                break;
            case AggregationType.Position:
                results = await _queryService.GetPositionKpiSummaryAsync(request.CycleId);
                break;
            default:
                return Result<IEnumerable<AggregatedKpiDto>>.Failure("Invalid aggregation type.");
        }

        return Result<IEnumerable<AggregatedKpiDto>>.Success(results);
    }
}
