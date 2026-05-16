using EPMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IMeetingNoteRepository
    {
        Task<IEnumerable<MeetingNote>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken);
        Task AddAsync(MeetingNote meetingNote, CancellationToken cancellationToken);
    }
}
