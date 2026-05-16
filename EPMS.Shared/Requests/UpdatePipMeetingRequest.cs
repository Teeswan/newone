using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Requests
{
    public class UpdatePipMeetingRequest
    {
        public int PipMeetingId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? DiscussionPoints { get; set; }
        public string ProgressStatus { get; set; } = string.Empty;
        public string? NextSteps { get; set; }
    }
}
