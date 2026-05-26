USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'FINAL FIX: Update Stored Procedure + Verify Data';
PRINT '============================================================';
GO

-- ============================================================
-- Step 1: Recreate sp_GetKpiByPosition to make 100% sure it's correct
-- ============================================================
PRINT '';
PRINT 'Step 1: Updating sp_GetKpiByPosition...';
GO

CREATE OR ALTER PROCEDURE sp_GetKpiByPosition
    @PositionId INT = NULL,
    @IsActive BIT = 1, 
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SELECT 
        pk.PositionKPIID AS PositionKpiId,
        k.KpiID AS KpiId,
        k.KPIName AS KpiName,
        k.Category AS Category,
        k.Unit AS Unit,
        k.IsActive AS IsActive,
        k.Direction AS Direction,
        pk.PositionID AS PositionId,
        pk.DefaultWeightPercent AS WeightPercent,
        pk.IsRequired AS IsRequired,
        COUNT(*) OVER() AS TotalCount
    FROM KPIs k
    INNER JOIN PositionKPIs pk ON k.KpiID = pk.KpiID
    WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)
      AND k.IsActive = @IsActive 
    ORDER BY k.KPIName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT '  ✓ sp_GetKpiByPosition updated!';
GO

-- ============================================================
-- Step 2: Verify PositionKPIs table has data
-- ============================================================
PRINT '';
PRINT 'Step 2: Checking PositionKPIs table data...';
GO

SELECT 
    PositionKPIID,
    PositionID,
    KpiID,
    DefaultWeightPercent,
    IsRequired
FROM PositionKPIs
ORDER BY PositionKPIID;

PRINT '';
PRINT '  NOTE: Make sure PositionKPIID column has values greater than 0!';
GO

-- ============================================================
-- Step 3: Test the stored procedure
-- ============================================================
PRINT '';
PRINT 'Step 3: Testing sp_GetKpiByPosition...';
GO

-- Test with no position (all)
EXEC sp_GetKpiByPosition @PositionId = NULL, @IsActive = 1, @PageNumber = 1, @PageSize = 100;
GO

PRINT '';
PRINT '============================================================';
PRINT 'DATABASE READY!';
PRINT '============================================================';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '1. STOP your application (EPMS.Api and EPMS.Blazor)';
PRINT '2. CLEAR REDIS CACHE (or restart Redis)';
PRINT '3. START your application again';
PRINT '';
PRINT 'This will clear the old cached data without PositionKpiId!';
PRINT '============================================================';
GO
