namespace EPMS.Shared.DTOs
{
    public class PipReportDto
    {
        public int PipId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalObjectives { get; set; }
        public int AchievedObjectives { get; set; }
        public int TotalMeetings { get; set; }
        public string? OverallGoal { get; set; }
    }
}
