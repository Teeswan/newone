USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Starting Complete KPI Fix Migration';
PRINT '============================================================';
GO

-- ============================================================
-- Step 1: Fix KPI column names from KPI_ID to KpiID
-- ============================================================
PRINT '';
PRINT 'Step 1: Fixing KPI column names...';

-- 1. Update KPIs table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KPIs') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'KPIs.KPI_ID', 'KpiID', 'COLUMN';
    PRINT '  ✓ Renamed KPIs.KPI_ID to KPIs.KpiID';
END
ELSE
BEGIN
    PRINT '  ✓ KPIs.KpiID already exists';
END

-- 2. Update DepartmentKpis table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DepartmentKpis') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'DepartmentKpis.KPI_ID', 'KpiID', 'COLUMN';
    PRINT '  ✓ Renamed DepartmentKpis.KPI_ID to DepartmentKpis.KpiID';
END
ELSE
BEGIN
    PRINT '  ✓ DepartmentKpis.KpiID already exists';
END

-- 3. Update EmployeeKpiAssignment table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpiAssignment') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'EmployeeKpiAssignment.KPI_ID', 'KpiID', 'COLUMN';
    PRINT '  ✓ Renamed EmployeeKpiAssignment.KPI_ID to EmployeeKpiAssignment.KpiID';
END
ELSE
BEGIN
    PRINT '  ✓ EmployeeKpiAssignment.KpiID already exists';
END

-- 4. Update PositionKPIs table (if exists)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PositionKPIs')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PositionKPIs') AND name = 'KPI_ID')
    BEGIN
        EXEC sp_rename 'PositionKPIs.KPI_ID', 'KpiID', 'COLUMN';
        PRINT '  ✓ Renamed PositionKPIs.KPI_ID to PositionKPIs.KpiID';
    END
    ELSE
    BEGIN
        PRINT '  ✓ PositionKPIs.KpiID already exists';
    END
END

-- ============================================================
-- Step 2: Recreate stored procedures with updated column names
-- ============================================================
PRINT '';
PRINT 'Step 2: Recreating stored procedures...';

-- 2.1 sp_GetKpiByPosition
PRINT '  Updating sp_GetKpiByPosition...';
GO

CREATE OR ALTER PROCEDURE sp_GetKpiByPosition
    @PositionId INT = NULL,
    @IsActive BIT = 1, 
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SELECT 
        k.*,
        pk.PositionID,
        pk.DefaultWeightPercent,
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
PRINT '  ✓ sp_GetKpiByPosition updated';

-- 2.2 sp_GetEmployeeKpiAssignment
PRINT '  Updating sp_GetEmployeeKpiAssignment...';
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeeKpiAssignment
    @EmployeeId INT,
    @CycleId INT
AS
BEGIN
    SELECT 
        AssignmentId,
        EmployeeID,
        CycleID,
        KpiID,
        KpiNameSnapshot,
        CategorySnapshot,
        UnitSnapshot,
        Direction,
        WeightPercent,
        TargetValue,
        ActualValue,
        KpiScore,
        WeightedScore,
        Status,
        SUM(WeightPercent) OVER(PARTITION BY CategorySnapshot) AS CategoryWeightSubtotal
    FROM EmployeeKpiAssignment
    WHERE EmployeeID = @EmployeeId AND CycleID = @CycleId
    ORDER BY CategorySnapshot, KpiNameSnapshot;
END
GO
PRINT '  ✓ sp_GetEmployeeKpiAssignment updated';

-- ============================================================
-- Complete
-- ============================================================
PRINT '';
PRINT '============================================================';
PRINT 'Complete KPI Fix Migration Finished Successfully!';
PRINT '============================================================';
PRINT '';
PRINT 'Now you can create Position KPIs, Department KPIs, and everything should work!';
GO
