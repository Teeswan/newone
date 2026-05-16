using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Requests
{
    public class CreatePipObjectiveRequest
    {
        public int PipId { get; set; }
        public string ObjectiveDescription { get; set; } = string.Empty;
        public string? SuccessCriteria { get; set; }
    }
}
