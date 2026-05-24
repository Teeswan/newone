using System;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Contexts;

namespace EPMS.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string entityName, string action, int? recordId, string details, int? employeeId = null)
    {
        var auditLog = new AuditLog
        {
            TableName = entityName,
            ActionType = action?.Length > 10 ? action.Substring(0, 10) : action,
            RecordId = recordId,
            OldData = null,
            NewData = details,
            ChangedByEmployeeId = (employeeId == 0) ? null : employeeId,
            ChangedAt = DateTimeOffset.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
