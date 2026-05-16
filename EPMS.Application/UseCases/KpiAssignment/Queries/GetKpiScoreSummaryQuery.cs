using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Queries;

public record GetKpiScoreSummaryQuery(int EmployeeId, int CycleId) : IRequest<Result<KpiScoreSummaryDto>>;

public class GetKpiScoreSummaryQueryHandler : IRequestHandler<GetKpiScoreSummaryQuery, Result<KpiScoreSummaryDto>>
{
    private readonly IKpiQueryService _queryService;

    public GetKpiScoreSummaryQueryHandler(IKpiQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<KpiScoreSummaryDto>> Handle(GetKpiScoreSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _queryService.GetKpiScoreSummaryAsync(request.EmployeeId, request.CycleId);
        if (summary == null) return Result<KpiScoreSummaryDto>.Failure("No KPI scores found for this employee and cycle.");

        var assignments = await _queryService.GetEmployeeKpiAssignmentAsync(request.EmployeeId, request.CycleId);
        summary.Assignments = (System.Collections.Generic.List<EmployeeKpiAssignmentDto>)assignments;

        return Result<KpiScoreSummaryDto>.Success(summary);
    }
}
