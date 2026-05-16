using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using EPMS.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Repositories;

public class KpiMasterRepository : IKpiMasterRepository
{
    private readonly AppDbContext _context;
    private readonly IDbConnectionFactory _connectionFactory;

    public KpiMasterRepository(AppDbContext context, IDbConnectionFactory connectionFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<KpiMaster?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "KPI id must be greater than zero.");
        }

        return await _context.KpiMasters
            .Include(k => k.Position)
            .FirstOrDefaultAsync(k => k.KpiId == id);
    }

    public async Task<IEnumerable<KpiMaster>> GetListByPositionAsync(int? positionId, bool isActive = true)
    {
        return await _context.KpiMasters
            .Where(k => k.PositionId == positionId && k.IsActive == isActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<KpiMaster>> GetGlobalKpisAsync(bool isActive = true)
    {
        return await _context.KpiMasters
            .Where(k => k.PositionId == null && k.IsActive == isActive)
            .ToListAsync();
    }

    public async Task AddAsync(KpiMaster kpi)
    {
        ArgumentNullException.ThrowIfNull(kpi);
        await _context.KpiMasters.AddAsync(kpi);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(KpiMaster kpi)
    {
        ArgumentNullException.ThrowIfNull(kpi);
        _context.KpiMasters.Update(kpi);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsKpiReferencedByActiveCycleAsync(int kpiId)
    {
        if (kpiId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kpiId), "KPI id must be greater than zero.");
        }

        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM EmployeeKpiAssignment WHERE KpiId = @KpiId AND Status = @Status";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { KpiId = kpiId, Status = "Active" });
        return count > 0;
    }

    public async Task<bool> ExistsDuplicateAsync(string name, string? category, int? positionId)
    {
        ArgumentNullException.ThrowIfNull(name);

        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT COUNT(1) FROM KPIs 
                           WHERE KPIName = @Name 
                           AND (Category = @Category OR (@Category IS NULL AND Category IS NULL)) 
                           AND (PositionId = @PositionId OR (@PositionId IS NULL AND PositionId IS NULL))
                           AND IsActive = 1";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name, Category = category, PositionId = positionId });
        return count > 0;
    }
}
