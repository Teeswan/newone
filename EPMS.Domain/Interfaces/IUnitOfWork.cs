using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMeetingRepository Meetings { get; }
        IMeetingNoteRepository MeetingNotes { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
