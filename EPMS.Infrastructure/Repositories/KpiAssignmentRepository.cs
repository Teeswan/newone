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

public class KpiAssignmentRepository : BaseRepository<EmployeeKpi, int>, IKpiAssignmentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public KpiAssignmentRepository(AppDbContext context, IDbConnectionFactory connectionFactory) : base(context)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<EmployeeKpi>> GetAssignmentsByEmployeeCycleAsync(int employeeId, int cycleId)
    {
        return await _dbSet
            .Where(e => e.EmployeeId == employeeId && e.CycleId == cycleId)
            .ToListAsync();
    }

    public override async Task<EmployeeKpi?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.EmployeeKpiId == id);
    }

    public async Task<EmployeeKpi?> GetSnapshotAsync(int employeeId, int cycleId, int kpiId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT * FROM EmployeeKpis 
                           WHERE EmployeeID = @EmployeeId AND CycleID = @CycleId AND KpiID = @KpiId";
        return await connection.QueryFirstOrDefaultAsync<EmployeeKpi>(sql, new { EmployeeId = employeeId, CycleId = cycleId, KpiId = kpiId });
    }

    public async Task AddRangeAsync(IEnumerable<EmployeeKpi> assignments)
    {
        ArgumentNullException.ThrowIfNull(assignments);
        await _dbSet.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(EmployeeKpi assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        await _dbSet.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }
}
