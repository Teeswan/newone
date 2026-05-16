using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Application.Settings
{
    public class MeetingSettings
    {
        public int JoinBufferMinutes { get; set; }
        public int StandardMeetingDurationMinutes { get; set; } // Used for calculating existing end times
    }
}
