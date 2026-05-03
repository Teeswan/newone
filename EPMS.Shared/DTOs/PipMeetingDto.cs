namespace EPMS.Shared.DTOs
{
    public class PipMeetingDto
    {
        public int PipMeetingId { get; set; }
        public int PipId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? DiscussionPoints { get; set; }
        public string? ProgressStatus { get; set; }
        public string? NextSteps { get; set; }
    }
}
