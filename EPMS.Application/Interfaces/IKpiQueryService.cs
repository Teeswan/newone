using EPMS.Shared.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IKpiQueryService
{
    Task<IEnumerable<PositionKpiDto>> GetKpiByPositionAsync(int? positionId, bool isActive = true, bool globalOnly = false);
    Task<PaginatedResult<PositionKpiDto>> GetPagedPositionKpiAsync(int? positionId, bool isActive, int pageNumber, int pageSize, string? searchTerm = null);
    Task<IEnumerable<AggregatedKpiDto>> GetDepartmentKpiSummaryAsync(int cycleId);
    Task<IEnumerable<AggregatedKpiDto>> GetTeamKpiSummaryAsync(int cycleId);
    Task<IEnumerable<AggregatedKpiDto>> GetPositionKpiSummaryAsync(int cycleId);
    Task<IEnumerable<dynamic>> GetKpiAuditLogAsync(string entityType, System.DateTime? dateFrom, System.DateTime? dateTo, int? userId);
}
