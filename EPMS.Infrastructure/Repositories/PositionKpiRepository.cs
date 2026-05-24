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

public class PositionKpiRepository : IPositionKpiRepository
{
    private readonly AppDbContext _context;
    private readonly IDbConnectionFactory _connectionFactory;

    public PositionKpiRepository(AppDbContext context, IDbConnectionFactory connectionFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<PositionKpi?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "KPI id must be greater than zero.");
        }

        return await _context.PositionKpis
            .Include(k => k.Position)
            .Include(k => k.Kpi)
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.PositionKpiId == id);
    }

    public async Task<IEnumerable<PositionKpi>> GetListByPositionAsync(int? positionId, bool isActive = true)
    {
        return await _context.PositionKpis
            .Include(k => k.Kpi)
            .Include(k => k.Position)
            .Where(k => k.PositionId == positionId && k.Kpi.IsActive == isActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<PositionKpi>> GetGlobalKpisAsync(bool isActive = true)
    {
        return await _context.PositionKpis
            .Include(k => k.Kpi)
            .Where(k => k.PositionId == null && k.Kpi.IsActive == isActive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(PositionKpi kpi)
    {
        ArgumentNullException.ThrowIfNull(kpi);
        await _context.PositionKpis.AddAsync(kpi);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PositionKpi kpi)
    {
        ArgumentNullException.ThrowIfNull(kpi);
        _context.PositionKpis.Update(kpi);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var kpi = await _context.PositionKpis.FindAsync(id);
        if (kpi != null)
        {
            _context.PositionKpis.Remove(kpi);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsKpiReferencedByActiveCycleAsync(int kpiId)
    {
        if (kpiId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kpiId), "KPI id must be greater than zero.");
        }

        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM EmployeeKpiAssignment WHERE KPI_ID = @KpiId AND Status = @Status";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { KpiId = kpiId, Status = "Active" });
        return count > 0;
    }

    public async Task<bool> ExistsDuplicateAsync(string name, string? category, int? positionId)
    {
        ArgumentNullException.ThrowIfNull(name);

        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT COUNT(1) FROM KPIs k
                           JOIN PositionKPIs pk ON k.KPI_ID = pk.KPI_ID
                           WHERE k.KPIName = @Name 
                           AND (k.Category = @Category OR (@Category IS NULL AND k.Category IS NULL)) 
                           AND (pk.PositionID = @PositionId OR (@PositionId IS NULL AND pk.PositionID IS NULL))
                           AND k.IsActive = 1";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name, Category = category, PositionId = positionId });
        return count > 0;
    }
}
