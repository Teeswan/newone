using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;

namespace EPMS.Infrastructure.Repositories;

public class MeetingNoteRepository : IMeetingNoteRepository
{
    private readonly AppDbContext _context;

    public MeetingNoteRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MeetingNote>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken)
    {
        return await _context.MeetingNotes
            .Where(n => n.MeetingId == meetingId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MeetingNote meetingNote, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(meetingNote);
        await _context.MeetingNotes.AddAsync(meetingNote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
