USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Creating EmployeeKpis Table';
PRINT '============================================================';
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKpis')
BEGIN
    CREATE TABLE EmployeeKpis (
        EmployeeKpiID INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID INT NOT NULL,
        TeamKpiID INT NOT NULL,
        TargetValue DECIMAL(18, 4) NOT NULL,
        WeightPercent DECIMAL(5, 2) NOT NULL,
        
        CONSTRAINT FK_EmployeeKpis_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
        CONSTRAINT FK_EmployeeKpis_TeamKpis FOREIGN KEY (TeamKpiID) REFERENCES TeamKpis(TeamKpiID)
    );
    PRINT '✅ EmployeeKpis table created';
END
ELSE
BEGIN
    PRINT 'ℹ️ EmployeeKpis table already exists';
END
GO

PRINT '============================================================';
PRINT 'EmployeeKpis Table Setup Complete!';
PRINT '============================================================';
GO
