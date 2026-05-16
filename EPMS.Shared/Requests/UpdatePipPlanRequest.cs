using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Requests
{
    public class UpdatePipPlanRequest
    {
        public int PipId { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? OverallGoal { get; set; }
    }
}
