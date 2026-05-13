using System.Net.Http.Json;
using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public class MeetingBlazorService : IMeetingBlazorService
{
    private readonly HttpClient _httpClient;

    public MeetingBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<MeetingDto>> GetManagerMeetingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<MeetingDto>>("api/meetings/manager") ?? Enumerable.Empty<MeetingDto>();
    }

    public async Task<IEnumerable<MeetingDto>> GetEmployeeMeetingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<MeetingDto>>("api/meetings/employee") ?? Enumerable.Empty<MeetingDto>();
    }

    public async Task<MeetingDto> GetMeetingByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<MeetingDto>($"api/meetings/{id}/details") ?? new MeetingDto();
    }

    public async Task ScheduleBulkMeetingsAsync(CreateMeetingDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/meetings/bulk-create", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateMeetingAsync(int id, UpdateMeetingDto request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/meetings/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task RescheduleMeetingAsync(int id, RescheduleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/meetings/{id}/reschedule", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task SkipMeetingAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/meetings/{id}/skip", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmMeetingAsync(int id, ConfirmMeetingDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/meetings/{id}/confirm", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<MeetingNoteDto> AddNoteAsync(int meetingId, AddMeetingNoteDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/meetings/{meetingId}/notes", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MeetingNoteDto>() ?? new MeetingNoteDto();
    }

    public async Task<IEnumerable<MeetingNoteDto>> GetNotesAsync(int meetingId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<MeetingNoteDto>>($"api/meetings/{meetingId}/notes") ?? Enumerable.Empty<MeetingNoteDto>();
    }

    public async Task<string> JoinMeetingAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/meetings/{id}/join", null);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.joinUrl ?? string.Empty;
    }

    public async Task EndMeetingAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/meetings/{id}/end", null);
        response.EnsureSuccessStatusCode();
    }
}
