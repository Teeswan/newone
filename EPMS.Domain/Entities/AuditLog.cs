using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class AuditLog
{
    public int AuditId { get; set; }
    public string? TableName { get; set; }
    public int? RecordId { get; set; }
    public string? ActionType { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public int? ChangedByEmployeeId { get; set; } // Now perfectly matches the database!
    public DateTimeOffset? ChangedAt { get; set; }

    public virtual Employee? ChangedByEmployee { get; set; }
}
