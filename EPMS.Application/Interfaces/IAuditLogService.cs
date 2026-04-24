using System.Threading.Tasks;

namespace EPMS.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string entityName, string action, int? recordId, string details, int? userId = null);
}
