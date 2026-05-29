USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Adding VersionNo to EmployeeKpis Table';
PRINT '============================================================';
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpis') AND name = 'VersionNo')
BEGIN
    ALTER TABLE EmployeeKpis ADD VersionNo INT NOT NULL DEFAULT 1;
    PRINT '  ✓ Added VersionNo column';
END
ELSE
BEGIN
    PRINT '  ℹ️ VersionNo column already exists';
END

GO
PRINT '============================================================';
PRINT 'EmployeeKpis Schema Update Complete!';
PRINT '============================================================';
GO
