using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Enums
{
    public static class MeetingProgressStatus
    {
        public const string OnTrack = "On Track";
        public const string AtRisk = "At Risk";
        public const string BehindSchedule = "Behind Schedule";
        public const string Completed = "Completed";
    }
}
