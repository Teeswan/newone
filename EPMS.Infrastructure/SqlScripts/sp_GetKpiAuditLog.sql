CREATE OR ALTER PROCEDURE sp_GetKpiAuditLog
    @EntityType NVARCHAR(100),
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL,
    @EmployeeID INT = NULL 
AS
BEGIN
    SELECT 
        [AuditId],
        [TableName],
        [RecordId],
        [ActionType],
        [OldData],
        [NewData],
        [ChangedByEmployeeId], 
        [ChangedAt]
    FROM AuditLogs
    WHERE TableName = @EntityType
      AND (@DateFrom IS NULL OR ChangedAt >= @DateFrom)
      AND (@DateTo IS NULL OR ChangedAt <= @DateTo)
      AND (@EmployeeID IS NULL OR ChangedByEmployeeId = @EmployeeID) 
    ORDER BY ChangedAt DESC;
END