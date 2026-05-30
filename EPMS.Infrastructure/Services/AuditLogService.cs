using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Contexts;
using EPMS.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AuditLogService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
            ChangedByEmployeeId = employeeId,
            ChangedAt = DateTimeOffset.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var logs = await _context.AuditLogs
            .Include(l => l.ChangedByEmployee)
            .OrderByDescending(l => l.ChangedAt)
            .ToListAsync();
        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string entityName)
    {
        var logs = await _context.AuditLogs
            .Include(l => l.ChangedByEmployee)
            .Where(l => l.TableName == entityName)
            .OrderByDescending(l => l.ChangedAt)
            .ToListAsync();
        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }
}
