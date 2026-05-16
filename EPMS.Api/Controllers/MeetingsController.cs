using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Shared.Constants;
using EPMS.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingsController(IMeetingService meetingService) => _meetingService = meetingService;

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return null;
        return userId;
    }

    [HttpPost("bulk-create")]
    // [HasPermission(Permissions.Meetings.Manage)] // Temporarily commented out for testing
    public async Task<IActionResult> BulkCreate([FromBody] CreateMeetingDto request, CancellationToken ct)
    {
        // Temporarily use managerId from request for testing (remove later!)
        if (!request.ManagerId.HasValue)
        {
            return BadRequest("ManagerId is required.");
        }
        await _meetingService.ScheduleBulkMeetingsAsync(request.ManagerId.Value, request, ct);
        return Ok(new { message = "Meetings scheduled successfully." });
    }

    [HttpGet("manager")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    // [HasPermission(Permissions.Meetings.View)] // Temporarily commented out for testing
    public async Task<IActionResult> GetManagerMeetings(CancellationToken ct)
    {
        // Temporarily hardcode managerId for testing (remove later!)
        int testManagerId = 3; // Change this to your test manager's ID!
        var meetings = await _meetingService.GetManagerMeetingsAsync(testManagerId, ct);
        return Ok(meetings);
    }

    [HttpGet("employee")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetEmployeeMeetings(CancellationToken ct)
    {
        // Temporarily hardcode employeeId for testing (remove later!)
        int testEmployeeId = 2; // Change this to your test employee's ID!
        var meetings = await _meetingService.GetEmployeeMeetingsAsync(testEmployeeId, ct);
        return Ok(meetings);
    }

    [HttpGet("{id}/details")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetMeetingDetails(int id, CancellationToken ct)
    {
        // Temporarily use a test user ID for testing
        int testUserId = 3; // Change to your test user ID!
        var meeting = await _meetingService.GetMeetingByIdAsync(id, testUserId, ct);
        return Ok(meeting);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Meetings.Manage)]
    public async Task<IActionResult> UpdateMeeting(int id, [FromBody] UpdateMeetingDto request, CancellationToken ct)
    {
        await _meetingService.UpdateMeetingAsync(id, request, ct);
        return Ok(new { message = "Meeting updated successfully." });
    }

    [HttpPost("{id}/reschedule")]
    [HasPermission(Permissions.Meetings.Manage)]
    public async Task<IActionResult> RescheduleMeeting(int id, [FromBody] RescheduleRequest request, CancellationToken ct)
    {
        await _meetingService.RescheduleMeetingAsync(id, request.UpdateData, request.Reason, ct);
        return Ok(new { message = "Meeting rescheduled successfully." });
    }

    [HttpPost("{id}/skip")]
    [HasPermission(Permissions.Meetings.Manage)]
    public async Task<IActionResult> SkipMeeting(int id, CancellationToken ct)
    {
        await _meetingService.SkipMeetingAsync(id, ct);
        return Ok(new { message = "Meeting skipped successfully." });
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmMeeting(int id, [FromBody] ConfirmMeetingDto request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();
        
        await _meetingService.ConfirmMeetingAsync(id, userId.Value, request, ct);
        return Ok(new { message = "Meeting confirmed successfully." });
    }

    [HttpPost("{id}/notes")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> AddNote(int id, [FromBody] AddMeetingNoteDto request, CancellationToken ct)
    {
        // Temporarily use a test user ID for testing
        int testUserId = 3; // Change to your test user ID!
        var note = await _meetingService.AddNoteAsync(id, request, testUserId, ct);
        return Ok(note);
    }

    [HttpGet("{id}/notes")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetNotes(int id, CancellationToken ct)
    {
        // Temporarily use a test user ID for testing
        int testUserId = 3; // Change to your test user ID!
        var notes = await _meetingService.GetMeetingNotesAsync(id, testUserId, ct);
        return Ok(notes);
    }

    [HttpPost("{id}/join")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> JoinMeeting(int id, CancellationToken ct)
    {
        await _meetingService.StartMeetingAsync(id, ct);
        return Ok(new { message = "Joined meeting successfully.", joinUrl = $"/meetings/{id}/room" });
    }

    [HttpPost("{id}/end")]
    [HasPermission(Permissions.Meetings.Manage)]
    public async Task<IActionResult> EndMeeting(int id, CancellationToken ct)
    {
        await _meetingService.EndMeetingAsync(id, ct);
        return Ok(new { message = "Meeting ended successfully." });
    }
}

public class RescheduleRequest
{
    public UpdateMeetingDto UpdateData { get; set; } = null!;
    public string Reason { get; set; } = string.Empty;
}