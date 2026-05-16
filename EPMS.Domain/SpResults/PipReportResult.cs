using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.SpResults
{
    public class PipReportResult
    {
        public int PipId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalObjectives { get; set; }
        public int AchievedObjectives { get; set; }
        public int TotalMeetings { get; set; }
        public string? OverallGoal { get; set; }
    }
}
