using EPMS.Shared.DTOs;

namespace EPMS.Application.Interfaces
{
    public interface IMeetingService
    {
        Task ScheduleBulkMeetingsAsync(int managerId, CreateMeetingDto request, CancellationToken ct);
        Task<IEnumerable<MeetingDto>> GetManagerMeetingsAsync(int managerId, CancellationToken ct);
        Task<IEnumerable<MeetingDto>> GetEmployeeMeetingsAsync(int employeeId, CancellationToken ct);
        Task<MeetingDto> GetMeetingByIdAsync(int meetingId, int userId, CancellationToken ct);
        Task UpdateMeetingAsync(int meetingId, UpdateMeetingDto request, CancellationToken ct);
        Task RescheduleMeetingAsync(int meetingId, UpdateMeetingDto request, string reason, CancellationToken ct);
        Task SkipMeetingAsync(int meetingId, CancellationToken ct);
        Task ConfirmMeetingAsync(int meetingId, int userId, ConfirmMeetingDto request, CancellationToken ct);
        Task<MeetingNoteDto> AddNoteAsync(int meetingId, AddMeetingNoteDto dto, int authorId, CancellationToken ct);
        Task<IEnumerable<MeetingNoteDto>> GetMeetingNotesAsync(int meetingId, int userId, CancellationToken ct);
        Task StartMeetingAsync(int meetingId, CancellationToken ct);
        Task EndMeetingAsync(int meetingId, CancellationToken ct);
    }
}
