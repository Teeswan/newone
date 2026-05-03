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

    public async Task ScheduleBulkMeetingsAsync(int managerId, BulkScheduleDto request, CancellationToken ct)
    {
        var existingMeetings = await _unitOfWork.Meetings.GetManagerDashboardAsync(managerId, ct);
        var currentStartTime = request.InitialStartTime;

        // Loop through each selected employee to calculate dynamic times
        foreach (var employeeId in request.EmployeeIds)
        {
            var calculatedEndTime = currentStartTime.AddMinutes(request.DurationMinutes);

            // Conflict Validation (No Database Changes needed!)
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

            // Create Meeting Entity
            var meeting = new OneOnOneMeeting
            {
                ManagerId = managerId,
                EmployeeId = employeeId,
                ScheduledDateTime = currentStartTime,
                MeetingStatus = "Scheduled",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Meetings.AddAsync(meeting, ct);

            // Calculate the start time for the next employee in the loop
            currentStartTime = calculatedEndTime.AddMinutes(request.BufferGapMinutes);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<MeetingDto>> GetManagerDashboardAsync(int managerId, CancellationToken ct)
    {
        var meetings = await _unitOfWork.Meetings.GetManagerDashboardAsync(managerId, ct);
        return _mapper.Map<IEnumerable<MeetingDto>>(meetings, opt => {
            opt.Items["JoinBufferMinutes"] = _settings.JoinBufferMinutes;
            opt.Items["StandardDuration"] = _settings.StandardMeetingDurationMinutes;
        });
    }

    public async Task<NoteDto> AddNoteAsync(int meetingId, CreateMeetingNoteDto dto, int authorId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        if (dto.IsPrivate)
        {
            // The "Enterprise Hack": Storing private notes in OneOnOneMeetings so employees can't see them in MeetingNotes table
            meeting.MeetingSummary = (meeting.MeetingSummary + "\n" + dto.NoteText).Trim();
            meeting.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            return new NoteDto { NoteText = dto.NoteText, CreatedAt = DateTime.UtcNow, AuthorId = authorId };
        }

        // Shared Notes go to the standard MeetingNotes table
        var note = new MeetingNote
        {
            MeetingId = meetingId,
            AuthorId = authorId,
            NoteText = dto.NoteText,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.MeetingNotes.AddAsync(note, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<NoteDto>(note);
    }

    public async Task StartMeetingAsync(int meetingId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        meeting.MeetingStatus = "Started";
        meeting.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task EndMeetingAsync(int meetingId, CancellationToken ct)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId, ct);
        if (meeting == null) throw new KeyNotFoundException("Meeting not found.");

        meeting.MeetingStatus = "Completed";
        meeting.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }
}