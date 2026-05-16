using EPMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Application.Exceptions
{
    public class MeetingConflictException : Exception
    {
        public IEnumerable<MeetingDto> ConflictingMeetings { get; }

        public MeetingConflictException(string message, IEnumerable<MeetingDto> conflictingMeetings)
            : base(message)
        {
            ConflictingMeetings = conflictingMeetings;
        }
    }
}
