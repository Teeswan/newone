using System;
using System.Collections.Generic;

namespace EPMS.Shared.DTOs
{
    public class MeetingDto
    {
        public int MeetingId { get; set; }
        public int? ManagerId { get; set; }
        public int? EmployeeId { get; set; }
        public string? ManagerName { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public string? Location { get; set; }
        public string? MeetingStatus { get; set; }
        public string? RescheduleReason { get; set; }
        public int? ParentMeetingId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DiscussionPoints { get; set; }
        public string? ActionItems { get; set; }
        public string? MeetingSummary { get; set; }
        public bool IsAdHoc { get; set; }
        public bool? ManagerConfirmed { get; set; }
        public DateTime? ManagerConfirmedAt { get; set; }
        public bool? EmployeeConfirmed { get; set; }
        public DateTime? EmployeeConfirmedAt { get; set; }
        public int? AppraisalCycleId { get; set; }
        public bool CanJoin { get; set; }
        public List<MeetingNoteDto> Notes { get; set; } = new();
    }

    public class MeetingNoteDto
    {
        public int NoteId { get; set; }
        public int? MeetingId { get; set; }
        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? NoteContent { get; set; }
        public string? NoteType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMeetingDto
    {
        public int? ManagerId { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
        public DateTime? ScheduledDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public int GapMinutes { get; set; }
        public string? Location { get; set; }
        public string? DiscussionPoints { get; set; }
        public bool IsAdHoc { get; set; }
        public int? AppraisalCycleId { get; set; }
    }

    public class UpdateMeetingDto
    {
        public DateTime? ScheduledDateTime { get; set; }
        public string? Location { get; set; }
        public string? DiscussionPoints { get; set; }
    }

    public class ConfirmMeetingDto
    {
        public bool IsConfirmed { get; set; }
        public string? DigitalSignature { get; set; }
    }

    public class AddMeetingNoteDto
    {
        public string? NoteContent { get; set; }
        public string? NoteType { get; set; }
    }
}
