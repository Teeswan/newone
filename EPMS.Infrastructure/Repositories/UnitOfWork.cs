using System;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;

namespace EPMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IMeetingRepository _meetings;
    private readonly IMeetingNoteRepository _meetingNotes;

    public UnitOfWork(AppDbContext context, IMeetingRepository meetings, IMeetingNoteRepository meetingNotes)
    {
        _context = context;
        _meetings = meetings;
        _meetingNotes = meetingNotes;
    }

    public IMeetingRepository Meetings => _meetings;
    public IMeetingNoteRepository MeetingNotes => _meetingNotes;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
