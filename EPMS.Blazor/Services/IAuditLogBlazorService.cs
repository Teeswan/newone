using EPMS.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services;

public interface IAuditLogBlazorService
{
    Task<IEnumerable<AuditLogDto>> GetAllAuditLogsAsync();
    Task<IEnumerable<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName);
}
