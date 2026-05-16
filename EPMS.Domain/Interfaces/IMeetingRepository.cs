using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces
{
    public interface IMeetingRepository
    {
        Task<OneOnOneMeeting?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<OneOnOneMeeting>> GetHistoryTimelineAsync(int rootMeetingId, CancellationToken cancellationToken);
        Task<IEnumerable<OneOnOneMeeting>> GetManagerDashboardAsync(int managerId, CancellationToken cancellationToken);
        Task<IEnumerable<OneOnOneMeeting>> GetEmployeeDashboardAsync(int employeeId, CancellationToken cancellationToken);
        Task AddAsync(OneOnOneMeeting meeting, CancellationToken cancellationToken);
    }
}