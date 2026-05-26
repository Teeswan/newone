USE [EmployeePerformance];
GO

-- Fix KPI column names from KPI_ID to KpiID (consistent pattern)

-- 1. Update KPIs table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KPIs') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'KPIs.KPI_ID', 'KpiID', 'COLUMN';
    PRINT 'Renamed KPIs.KPI_ID to KPIs.KpiID';
END
ELSE
BEGIN
    PRINT 'KPIs.KpiID already exists, skipping rename';
END

-- 2. Update DepartmentKpis table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DepartmentKpis') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'DepartmentKpis.KPI_ID', 'KpiID', 'COLUMN';
    PRINT 'Renamed DepartmentKpis.KPI_ID to DepartmentKpis.KpiID';
END
ELSE
BEGIN
    PRINT 'DepartmentKpis.KpiID already exists, skipping rename';
END

-- 3. Update EmployeeKpiAssignment table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpiAssignment') AND name = 'KPI_ID')
BEGIN
    EXEC sp_rename 'EmployeeKpiAssignment.KPI_ID', 'KpiID', 'COLUMN';
    PRINT 'Renamed EmployeeKpiAssignment.KPI_ID to EmployeeKpiAssignment.KpiID';
END
ELSE
BEGIN
    PRINT 'EmployeeKpiAssignment.KpiID already exists, skipping rename';
END

-- 4. Update PositionKPIs table (if exists)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PositionKPIs')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PositionKPIs') AND name = 'KPI_ID')
    BEGIN
        EXEC sp_rename 'PositionKPIs.KPI_ID', 'KpiID', 'COLUMN';
        PRINT 'Renamed PositionKPIs.KPI_ID to PositionKPIs.KpiID';
    END
    ELSE
    BEGIN
        PRINT 'PositionKPIs.KpiID already exists, skipping rename';
    END
END

PRINT 'All KPI column name updates completed!';
GO
