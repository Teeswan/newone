using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Queries;

public record GetEmployeeKpiAssignmentQuery(int EmployeeId, int CycleId) : IRequest<Result<IEnumerable<EmployeeKpiAssignmentDto>>>;

public class GetEmployeeKpiAssignmentQueryHandler : IRequestHandler<GetEmployeeKpiAssignmentQuery, Result<IEnumerable<EmployeeKpiAssignmentDto>>>
{
    private readonly IKpiQueryService _queryService;
    private readonly IKpiCacheService _cacheService;

    public GetEmployeeKpiAssignmentQueryHandler(IKpiQueryService queryService, IKpiCacheService cacheService)
    {
        _queryService = queryService;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<EmployeeKpiAssignmentDto>>> Handle(GetEmployeeKpiAssignmentQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"kpiassign:emp:{request.EmployeeId}:cyc:{request.CycleId}";
        var cached = await _cacheService.GetAsync<IEnumerable<EmployeeKpiAssignmentDto>>(cacheKey);
        if (cached != null) return Result<IEnumerable<EmployeeKpiAssignmentDto>>.Success(cached);

        var assignments = await _queryService.GetEmployeeKpiAssignmentAsync(request.EmployeeId, request.CycleId);

        await _cacheService.SetAsync(cacheKey, assignments, TimeSpan.FromMinutes(15));
        return Result<IEnumerable<EmployeeKpiAssignmentDto>>.Success(assignments);
    }
}
