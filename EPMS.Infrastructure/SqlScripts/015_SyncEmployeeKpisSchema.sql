USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Syncing EmployeeKpis table with Application Model';
PRINT '============================================================';
GO

-- 1. Rename table to plural if it is singular
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKpi')
AND NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKpis')
BEGIN
    EXEC sp_rename 'EmployeeKpi', 'EmployeeKpis';
    PRINT '  ✓ Renamed EmployeeKpi to EmployeeKpis';
END
GO

-- 2. Rename columns to match the new model
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'EmployeeTarget')
BEGIN
    EXEC sp_rename 'EmployeeKpis.EmployeeTarget', 'TargetValue', 'COLUMN';
    PRINT '  ✓ Renamed EmployeeTarget to TargetValue';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'Weight')
BEGIN
    EXEC sp_rename 'EmployeeKpis.Weight', 'WeightPercent', 'COLUMN';
    PRINT '  ✓ Renamed Weight to WeightPercent';
END
GO

-- 3. Ensure all other required columns exist (for snapshots etc)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'CycleID')
    ALTER TABLE EmployeeKpis ADD CycleID INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'KpiID')
    ALTER TABLE EmployeeKpis ADD KpiID INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'KpiNameSnapshot')
    ALTER TABLE EmployeeKpis ADD KpiNameSnapshot NVARCHAR(255) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'CategorySnapshot')
    ALTER TABLE EmployeeKpis ADD CategorySnapshot NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'UnitSnapshot')
    ALTER TABLE EmployeeKpis ADD UnitSnapshot NVARCHAR(50) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'Direction')
    ALTER TABLE EmployeeKpis ADD Direction INT NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'Status')
    ALTER TABLE EmployeeKpis ADD Status NVARCHAR(20) NOT NULL DEFAULT 'Active';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'IsAdHoc')
    ALTER TABLE EmployeeKpis ADD IsAdHoc BIT NOT NULL DEFAULT 0;
GO

-- 4. Update the stored procedure to use the correct table name
PRINT 'Updating sp_GetEmployeeKpiAssignment...';
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeeKpiAssignment
    @EmployeeId INT,
    @CycleId INT
AS
BEGIN
    SELECT 
        EmployeeKpiID AS AssignmentId,
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
    FROM EmployeeKpis
    WHERE EmployeeID = @EmployeeId AND (@CycleId = 0 OR CycleID = @CycleId)
    ORDER BY CategorySnapshot, KpiNameSnapshot;
END
GO

PRINT '============================================================';
PRINT 'Migration Complete!';
PRINT '============================================================';
GO
