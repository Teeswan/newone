using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IKpiQueryService
{
    Task<IEnumerable<KpiMasterDto>> GetKpisByPositionAsync(int? positionId, bool isActive = true);
    Task<IEnumerable<EmployeeKpiAssignmentDto>> GetEmployeeKpiAssignmentAsync(int employeeId, int cycleId);
    Task<KpiScoreSummaryDto?> GetKpiScoreSummaryAsync(int employeeId, int cycleId);
    Task<IEnumerable<dynamic>> GetKpiAuditLogAsync(string entityType, System.DateTime? dateFrom, System.DateTime? dateTo, int? userId);
}
