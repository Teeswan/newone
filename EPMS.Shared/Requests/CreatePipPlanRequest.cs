using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Requests
{
    public class CreatePipPlanRequest
    {
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? OverallGoal { get; set; }
        public List<CreatePipObjectiveRequest> Objectives { get; set; } = new();
    }
}
