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
    public sealed class PipMeetingRepository : IPipMeetingRepository
    {
        private readonly AppDbContext _db;

        public PipMeetingRepository(AppDbContext db) => _db = db;

        public async Task<PipMeeting?> GetByIdAsync(int meetingId, CancellationToken ct = default) =>
            await _db.PipMeetings.FindAsync([meetingId], ct);

        public async Task<IEnumerable<PipMeeting>> GetByPipIdAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipMeetings
                .AsNoTracking()
                .Where(m => m.Pipid == pipId)
                .OrderBy(m => m.MeetingDate)
                .ToListAsync(ct);

        public async Task<PipMeeting> CreateAsync(PipMeeting meeting, CancellationToken ct = default)
        {
            _db.PipMeetings.Add(meeting);
            await _db.SaveChangesAsync(ct);
            return meeting;
        }

        public async Task<PipMeeting> UpdateAsync(PipMeeting meeting, CancellationToken ct = default)
        {
            _db.PipMeetings.Update(meeting);
            await _db.SaveChangesAsync(ct);
            return meeting;
        }

        public async Task<bool> DeleteAsync(int meetingId, CancellationToken ct = default)
        {
            var meeting = await _db.PipMeetings.FindAsync([meetingId], ct);
            if (meeting is null) return false;
            _db.PipMeetings.Remove(meeting);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<PipMeeting?> GetLatestMeetingAsync(int pipId, CancellationToken ct = default) =>
            await _db.PipMeetings
                .AsNoTracking()
                .Where(m => m.Pipid == pipId)
                .OrderByDescending(m => m.MeetingDate)
                .FirstOrDefaultAsync(ct);
    }
}
