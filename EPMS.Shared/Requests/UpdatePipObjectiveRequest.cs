using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Requests
{
    public class UpdatePipObjectiveRequest
    {
        public int ObjectiveId { get; set; }
        public bool IsAchieved { get; set; }
        public string? ReviewComments { get; set; }
        public string? SuccessCriteria { get; set; }
    }
}
