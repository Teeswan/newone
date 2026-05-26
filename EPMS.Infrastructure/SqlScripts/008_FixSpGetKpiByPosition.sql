USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Fixing sp_GetKpiByPosition to return PositionKpiId';
PRINT '============================================================';
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
    LEFT JOIN PositionKPIs pk ON k.KpiID = pk.KpiID
    WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)
      AND k.IsActive = @IsActive 
    ORDER BY k.KPIName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT '';
PRINT '✓ sp_GetKpiByPosition updated to return PositionKpiId!';
PRINT '';
PRINT 'Now restart your application to clear the cache!';
PRINT '============================================================';
GO
