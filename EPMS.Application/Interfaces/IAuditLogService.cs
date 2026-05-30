using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string entityName, string action, int? recordId, string details, int? employeeId = null);
    Task<IEnumerable<AuditLogDto>> GetAllAsync();
    Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string entityName);
}
