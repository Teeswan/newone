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
    public sealed class PipObjectiveRepository : IPipObjectiveRepository
    {
        private readonly AppDbContext _db;

        public PipObjectiveRepository(AppDbContext db) => _db = db;

        public async Task<PipObjective?> GetByIdAsync(int objectiveId, CancellationToken ct = default) =>
            await _db.PipObjectives.FindAsync([objectiveId], ct);

        public async Task<IEnumerable<PipObjective>> GetByPipIdAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipObjectives
                .AsNoTracking()
                .Where(o => o.Pipid == pipId)
                .ToListAsync(ct);

        public async Task<PipObjective> CreateAsync(PipObjective objective, CancellationToken ct = default)
        {
            _db.PipObjectives.Add(objective);
            await _db.SaveChangesAsync(ct);
            return objective;
        }

        public async Task<PipObjective> UpdateAsync(PipObjective objective, CancellationToken ct = default)
        {
            _db.PipObjectives.Update(objective);
            await _db.SaveChangesAsync(ct);
            return objective;
        }

        public async Task<bool> DeleteAsync(int objectiveId, CancellationToken ct = default)
        {
            var obj = await _db.PipObjectives.FindAsync([objectiveId], ct);
            if (obj is null) return false;
            _db.PipObjectives.Remove(obj);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<int> GetAchievedCountByPipIdAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipObjectives
                .CountAsync(o => o.Pipid == pipId && o.IsAchieved == true, ct);
    }
}
