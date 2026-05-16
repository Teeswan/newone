using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public interface IMeetingBlazorService
{
    Task<IEnumerable<MeetingDto>> GetManagerMeetingsAsync();
    Task<IEnumerable<MeetingDto>> GetEmployeeMeetingsAsync();
    Task<MeetingDto> GetMeetingByIdAsync(int id);
    Task ScheduleBulkMeetingsAsync(CreateMeetingDto request);
    Task UpdateMeetingAsync(int id, UpdateMeetingDto request);
    Task RescheduleMeetingAsync(int id, RescheduleRequest request);
    Task SkipMeetingAsync(int id);
    Task ConfirmMeetingAsync(int id, ConfirmMeetingDto request);
    Task<MeetingNoteDto> AddNoteAsync(int meetingId, AddMeetingNoteDto request);
    Task<IEnumerable<MeetingNoteDto>> GetNotesAsync(int meetingId);
    Task<string> JoinMeetingAsync(int id);
    Task EndMeetingAsync(int id);
}

public class RescheduleRequest
{
    public UpdateMeetingDto UpdateData { get; set; } = null!;
    public string Reason { get; set; } = string.Empty;
}
