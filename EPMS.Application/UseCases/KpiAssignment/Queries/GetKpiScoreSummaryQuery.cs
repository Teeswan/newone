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

        // Set Performance Label and Promotion Eligibility based on Score
        if (summary.TotalPerformanceScore >= 86)
        {
            summary.PerformanceLabel = "Outstanding";
            summary.PromotionEligibility = "Highly Eligible";
        }
        else if (summary.TotalPerformanceScore >= 71)
        {
            summary.PerformanceLabel = "Exceeds Expectations";
            summary.PromotionEligibility = "Eligible";
        }
        else if (summary.TotalPerformanceScore >= 60)
        {
            summary.PerformanceLabel = "Meets Expectations";
            summary.PromotionEligibility = "Potentially Eligible";
        }
        else if (summary.TotalPerformanceScore >= 40)
        {
            summary.PerformanceLabel = "Needs Improvement";
            summary.PromotionEligibility = "Not Eligible";
        }
        else
        {
            summary.PerformanceLabel = "Poor Performance";
            summary.PromotionEligibility = "Not Eligible / PIP Required";
        }

        return Result<KpiScoreSummaryDto>.Success(summary);
    }
}
