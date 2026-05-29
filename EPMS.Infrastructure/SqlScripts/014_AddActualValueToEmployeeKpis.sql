USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Adding Actual and Score Columns to EmployeeKpis Table';
PRINT '============================================================';
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'ActualValue')
BEGIN
    ALTER TABLE EmployeeKpis ADD ActualValue DECIMAL(18, 4) NULL;
    PRINT '  ✓ Added ActualValue column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'KpiScore')
BEGIN
    ALTER TABLE EmployeeKpis ADD KpiScore DECIMAL(18, 4) NULL;
    PRINT '  ✓ Added KpiScore column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'WeightedScore')
BEGIN
    ALTER TABLE EmployeeKpis ADD WeightedScore DECIMAL(18, 4) NULL;
    PRINT '  ✓ Added WeightedScore column';
END

GO
PRINT '============================================================';
PRINT 'EmployeeKpis Schema Update Complete!';
PRINT '============================================================';
GO
