using EPMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IPipMeetingRepository
    {
        Task<PipMeeting?> GetByIdAsync(int meetingId, CancellationToken ct = default);
        Task<IEnumerable<PipMeeting>> GetByPipIdAsync(int pipId, CancellationToken ct = default);
        Task<PipMeeting> CreateAsync(PipMeeting meeting, CancellationToken ct = default);
        Task<PipMeeting> UpdateAsync(PipMeeting meeting, CancellationToken ct = default);
        Task<bool> DeleteAsync(int meetingId, CancellationToken ct = default);
        Task<PipMeeting?> GetLatestMeetingAsync(int pipId, CancellationToken ct = default);
    }
}
