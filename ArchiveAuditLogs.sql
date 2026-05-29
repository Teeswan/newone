-- =============================================
-- Author: EPMS Agent
-- Create date: 2026-05-28
-- Description: Periodic Data Archiving for AuditLogs
-- =============================================
USE EmployeePerformance;
GO

CREATE OR ALTER PROCEDURE sp_ArchiveAuditLogs
    @RetentionYears INT = 2
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ArchiveDate DATETIMEOFFSET = DATEADD(YEAR, -@RetentionYears, SYSDATETIMEOFFSET());
    
    -- 1. Create Archive table if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs_Archive')
    BEGIN
        SELECT TOP 0 * INTO AuditLogs_Archive FROM AuditLogs;
        PRINT 'Created AuditLogs_Archive table.';
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- 2. Move old logs to Archive table
        INSERT INTO AuditLogs_Archive
        SELECT * FROM AuditLogs
        WHERE ChangedAt < @ArchiveDate;

        DECLARE @MovedCount INT = @@ROWCOUNT;

        -- 3. Delete from primary table
        DELETE FROM AuditLogs
        WHERE ChangedAt < @ArchiveDate;

        COMMIT TRANSACTION;
        PRINT CAST(@MovedCount AS VARCHAR) + ' rows archived successfully.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
