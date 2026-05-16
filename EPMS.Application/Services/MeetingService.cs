using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Application.Settings;
using EPMS.Application.Exceptions;

namespace EPMS.Application.Services;

public class MeetingService : IMeetingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MeetingSettings _settings;

    public MeetingService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<MeetingSettings> settings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _settings = settings.Value;
    }

    public async Task ScheduleBulkMeetingsAsync(int managerId, CreateMeetingDto request, CancellationToken ct)
    {
        var existingMeetings = await _unitOfWork.Meetings.GetManagerDashboardAsync(managerId, ct);
        var currentStartTime = request.ScheduledDateTime ?? DateTime.UtcNow;

        foreach (var employeeId in request.EmployeeIds)
        {
            var calculatedEndTime = currentStartTime.AddMinutes(request.DurationMinutes);

            if (!request.IsAdHoc)
            {
                var conflicts = existingMeetings.Where(m =>
                    m.ScheduledDateTime.HasValue &&
                    currentStartTime < m.ScheduledDateTime.Value.AddMinutes(_settings.StandardMeetingDurationMinutes) &&
                    calculatedEndTime > m.ScheduledDateTime.Value).ToList();

                if (conflicts.Any())
                {
                    var conflictDtos = _mapper.Map<IEnumerable<MeetingDto>>(conflicts, opt => {
                        opt.Items["StandardDuration"] = _settings.StandardMeetingDurationMinutes;
                        opt.Items["JoinBufferMinutes"] = _settings.JoinBufferMinutes;
                    });
                    throw new MeetingConflictException($"Conflict detected for Employee {employeeId} at {currentStartTime}.", conflictDtos);
                }
            }

            var meeting = new OneOnOneMeeting
            {
                ManagerId = managerId,
                EmployeeId = employeeId,
                ScheduledDateTime = currentStartTime,
                Location = request.Location,
                DiscussionPoints = request.DiscussionPoints,
                MeetingStatus = "Scheduled",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Meetings.AddAsync(meeting, ct);
            currentStartTime = calculatedEndTime.AddMinutes(request.GapMinutes);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<MeetingDto>> GetManagerMeetingsAsync(int managerId, CancellationToken ct)
    {
        var meetings = await _unitOfWork.Meetings.GetManagerDashboardAsync(managerId, ct);
        return _mapper.Map<IEnumerable<MeetingDto>>(meetings, opt => {
            opt.Items["StandardDuration"] = _settings.StandardMeetingDurationMinutes;
            opt.Items["JoinBufferMinutes"] = _settings.JoinBufferMinutes;
        });
    }

    public async Task<IEnumerable<MeetingDto>> GetEmployeeMeetingsAsync(int employeeId, CancellationToken ct)
    {
        var meetings = await _unitOfWork.Meetings.GetEmployeeDashboardAsync(employeeId, ct);
        return _mapper.Map<IEnumerable<MeetingDto>>(meetings, opt => {
            opt.Items["StandardDuration"] = _settings.StandardMeetingDurationMinutes;
            opt.Items["JoinBufferMinutes"] = _settings.JoinBufferMinutes;
        });
    }

    public async Task<MeetingDto> GetMeetingByIdAsync(int meetingId, int userId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        if (meeting.ManagerId != userId && meeting.EmployeeId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this meeting.");

        var dto = _mapper.Map<MeetingDto>(meeting, opt => {
            opt.Items["StandardDuration"] = _settings.StandardMeetingDurationMinutes;
            opt.Items["JoinBufferMinutes"] = _settings.JoinBufferMinutes;
        });
        
        dto.Notes = (await GetMeetingNotesAsync(meetingId, userId, ct)).ToList();
        return dto;
    }

    public async Task UpdateMeetingAsync(int meetingId, UpdateMeetingDto request, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        if (request.ScheduledDateTime.HasValue)
            meeting.ScheduledDateTime = request.ScheduledDateTime.Value;
        if (!string.IsNullOrEmpty(request.Location))
            meeting.Location = request.Location;
        if (!string.IsNullOrEmpty(request.DiscussionPoints))
            meeting.DiscussionPoints = request.DiscussionPoints;
        
        meeting.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RescheduleMeetingAsync(int meetingId, UpdateMeetingDto request, string reason, CancellationToken ct)
    {
        var originalMeeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (originalMeeting == null) throw new KeyNotFoundException("Meeting not found.");

        var rescheduledMeeting = new OneOnOneMeeting
        {
            ManagerId = originalMeeting.ManagerId,
            EmployeeId = originalMeeting.EmployeeId,
            ScheduledDateTime = request.ScheduledDateTime ?? originalMeeting.ScheduledDateTime,
            Location = request.Location ?? originalMeeting.Location,
            DiscussionPoints = request.DiscussionPoints ?? originalMeeting.DiscussionPoints,
            ParentMeetingId = originalMeeting.MeetingId,
            MeetingStatus = "Rescheduled",
            RescheduleReason = reason,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        originalMeeting.MeetingStatus = "Rescheduled";
        originalMeeting.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Meetings.AddAsync(rescheduledMeeting, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task SkipMeetingAsync(int meetingId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        meeting.MeetingStatus = "Skipped";
        meeting.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ConfirmMeetingAsync(int meetingId, int userId, ConfirmMeetingDto request, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        if (meeting.ManagerId == userId)
        {
            meeting.MeetingSummary = (meeting.MeetingSummary + $"\nManager confirmed at {DateTime.UtcNow}").Trim();
        }
        else if (meeting.EmployeeId == userId)
        {
            meeting.MeetingSummary = (meeting.MeetingSummary + $"\nEmployee confirmed at {DateTime.UtcNow}").Trim();
        }
        else
        {
            throw new UnauthorizedAccessException("You are not authorized to confirm this meeting.");
        }

        meeting.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<MeetingNoteDto> AddNoteAsync(int meetingId, AddMeetingNoteDto dto, int authorId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        var note = new MeetingNote
        {
            MeetingId = meetingId,
            AuthorId = authorId,
            NoteText = dto.NoteContent ?? string.Empty,
            NoteType = dto.NoteType ?? "Shared",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.MeetingNotes.AddAsync(note, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<MeetingNoteDto>(note);
    }

    public async Task<IEnumerable<MeetingNoteDto>> GetMeetingNotesAsync(int meetingId, int userId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        if (meeting.ManagerId != userId && meeting.EmployeeId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view these notes.");

        var notes = await _unitOfWork.MeetingNotes.GetByMeetingIdAsync(meetingId, ct);
        return _mapper.Map<IEnumerable<MeetingNoteDto>>(notes);
    }

    public async Task StartMeetingAsync(int meetingId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        meeting.MeetingStatus = "InProgress";
        meeting.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task EndMeetingAsync(int meetingId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        meeting.MeetingStatus = "PendingConfirmation";
        meeting.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }
}