using System;

namespace EPMS.Shared.DTOs;

public class AuditLogDto
{
    public int AuditId { get; set; }
    public string? TableName { get; set; }
    public int? RecordId { get; set; }
    public string? ActionType { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public int? ChangedByEmployeeId { get; set; }
    public string? ChangedByEmployeeName { get; set; }
    public DateTimeOffset? ChangedAt { get; set; }
}
