using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IKpiQueryService
{
    Task<IEnumerable<PositionKpiDto>> GetKpiByPositionAsync(int? positionId, bool isActive = true);
    Task<IEnumerable<EmployeeKpiAssignmentDto>> GetEmployeeKpiAssignmentAsync(int employeeId, int cycleId);
    Task<KpiScoreSummaryDto?> GetKpiScoreSummaryAsync(int employeeId, int cycleId);
    Task<IEnumerable<AggregatedKpiDto>> GetDepartmentKpiSummaryAsync(int cycleId);
    Task<IEnumerable<AggregatedKpiDto>> GetTeamKpiSummaryAsync(int cycleId);
    Task<IEnumerable<AggregatedKpiDto>> GetPositionKpiSummaryAsync(int cycleId);
    Task<IEnumerable<dynamic>> GetKpiAuditLogAsync(string entityType, System.DateTime? dateFrom, System.DateTime? dateTo, int? userId);
}
