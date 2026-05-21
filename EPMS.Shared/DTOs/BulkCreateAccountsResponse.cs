namespace EPMS.Shared.DTOs;

public record BulkCreateAccountsResponse(int CreatedCount, int SkippedCount, string Message);
