using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Infrastructure.Repositories
{
    public sealed class PipPlanRepository : IPipPlanRepository
    {
        private readonly AppDbContext _db;

        public PipPlanRepository(AppDbContext db) => _db = db;

        public async Task<PipPlan?> GetByIdAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipPlans.FindAsync([pipId], ct);

        public async Task<PipPlan?> GetByIdWithDetailsAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipPlans
                .AsNoTracking()
                .Include(p => p.Employee)
                .Include(p => p.Manager)
                .Include(p => p.PipObjectives)
                .Include(p => p.PipMeetings.OrderBy(m => m.MeetingDate))
                .FirstOrDefaultAsync(p => p.Pipid == pipId, ct);

        public async Task<IEnumerable<PipPlan>> GetAllAsync(CancellationToken ct = default) =>
            await _db.PipPlans
                .AsNoTracking()
                .Include(p => p.Employee)
                .Include(p => p.Manager)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);

        public async Task<IEnumerable<PipPlan>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default) =>
            await _db.PipPlans
                .AsNoTracking()
                .Where(p => p.EmployeeId == employeeId)
                .Include(p => p.PipObjectives)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);

        public async Task<IEnumerable<PipPlan>> GetByManagerIdAsync(int managerId, CancellationToken ct = default) =>
            await _db.PipPlans
                .AsNoTracking()
                .Where(p => p.ManagerId == managerId)
                .Include(p => p.Employee)
                .Include(p => p.PipObjectives)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);

        public async Task<bool> HasActivePipAsync(int employeeId, CancellationToken ct = default) =>
            await _db.PipPlans
                .AnyAsync(p => p.EmployeeId == employeeId && p.Status == "Active", ct);

        public async Task<PipPlan> CreateAsync(PipPlan plan, CancellationToken ct = default)
        {
            _db.PipPlans.Add(plan);
            await _db.SaveChangesAsync(ct);
            return plan;
        }

        public async Task<PipPlan> UpdateAsync(PipPlan plan, CancellationToken ct = default)
        {
            _db.PipPlans.Update(plan);
            await _db.SaveChangesAsync(ct);
            return plan;
        }

        public async Task<bool> DeleteAsync(int pipId, CancellationToken ct = default)
        {
            var plan = await _db.PipPlans.FindAsync([pipId], ct);
            if (plan is null) return false;
            _db.PipPlans.Remove(plan);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
