using System;
using System.Collections.Generic;
using System.Linq;
namespace EPMS.Shared.DTOs
{
    public class PipPlanDto
    {
        public int PipId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? OverallGoal { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PipObjectiveDto> Objectives { get; set; } = new();
        public List<PipMeetingDto> Meetings { get; set; } = new();
    }
}
