CREATE PROCEDURE sp_GetKpiAuditLog
    @EntityType NVARCHAR(100),
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL,
    @UserId INT = NULL
AS
BEGIN
    SELECT * 
    FROM AuditLogs
    WHERE TableName = @EntityType
      AND (@DateFrom IS NULL OR ChangedAt >= @DateFrom)
      AND (@DateTo IS NULL OR ChangedAt <= @DateTo)
      AND (@UserId IS NULL OR ChangedByUserId = @UserId)
    ORDER BY ChangedAt DESC;
END
GO
