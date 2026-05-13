using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class MeetingNote
{
    public int NoteId { get; set; }

    public int MeetingId { get; set; }

    public int AuthorId { get; set; }

    public string NoteText { get; set; } = null!;

    public string? NoteType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Employee? Author { get; set; }

    public virtual OneOnOneMeeting? Meeting { get; set; }
}
