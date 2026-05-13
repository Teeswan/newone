using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PipMeeting
{
    public int PipMeetingId { get; set; }

    public int? Pipid { get; set; }
    public DateTime MeetingDate { get; set; }
    public string? DiscussionPoints { get; set; }

    public string? ProgressStatus { get; set; }
    public string? NextSteps { get; set; }

    public virtual PipPlan? Pip { get; set; }
}
