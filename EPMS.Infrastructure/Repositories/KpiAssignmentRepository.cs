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

public class KpiAssignmentRepository : IKpiAssignmentRepository
{
    private readonly AppDbContext _context;
    private readonly IDbConnectionFactory _connectionFactory;

    public KpiAssignmentRepository(AppDbContext context, IDbConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<EmployeeKpiAssignment>> GetAssignmentsByEmployeeCycleAsync(int employeeId, int cycleId)
    {
        return await _context.EmployeeKpiAssignments
            .Where(e => e.EmployeeId == employeeId && e.CycleId == cycleId)
            .ToListAsync();
    }

    public async Task<EmployeeKpiAssignment?> GetByIdAsync(int assignmentId)
    {
        return await _context.EmployeeKpiAssignments
            .FirstOrDefaultAsync(e => e.AssignmentId == assignmentId);
    }

    public async Task<EmployeeKpiAssignment?> GetSnapshotAsync(int employeeId, int cycleId, int kpiId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT * FROM EmployeeKpiAssignment 
                           WHERE EmployeeID = @EmployeeId AND CycleID = @CycleId AND KPI_ID = @KpiId";
        return await connection.QueryFirstOrDefaultAsync<EmployeeKpiAssignment>(sql, new { EmployeeId = employeeId, CycleId = cycleId, KpiId = kpiId });
    }

    public async Task AddRangeAsync(IEnumerable<EmployeeKpiAssignment> assignments)
    {
        await _context.EmployeeKpiAssignments.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(EmployeeKpiAssignment assignment)
    {
        await _context.EmployeeKpiAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmployeeKpiAssignment assignment)
    {
        _context.EmployeeKpiAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }
}
