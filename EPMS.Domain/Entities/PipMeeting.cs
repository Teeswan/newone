using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class PipMeeting
{
    public int PipMeetingId { get; set; }

    public int? Pipid { get; set; }

    public string? ProgressStatus { get; set; }

    public virtual PipPlan? Pip { get; set; }
}
