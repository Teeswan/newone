USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Adding Missing Columns to EmployeeKpis Table';
PRINT '============================================================';
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'IsActive')
BEGIN
    ALTER TABLE EmployeeKpis ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT '  ✓ Added IsActive column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE EmployeeKpis ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    PRINT '  ✓ Added CreatedAt column';
END

GO
PRINT '============================================================';
PRINT 'EmployeeKpis Schema Update Complete!';
PRINT '============================================================';
GO
