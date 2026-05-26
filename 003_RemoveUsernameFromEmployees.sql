-- Remove Username column from Employees table
USE EmployeePerformance;
GO

-- Drop the unique index on Username first (check all possible names)
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_EmployeeUsername' AND object_id = OBJECT_ID('Employees'))
BEGIN
    DROP INDEX UQ_EmployeeUsername ON Employees;
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ__Employee__536C85E4743578EF' AND object_id = OBJECT_ID('Employees'))
BEGIN
    DROP INDEX UQ__Employee__536C85E4743578EF ON Employees;
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Employees_Username' AND object_id = OBJECT_ID('Employees'))
BEGIN
    DROP INDEX UQ_Employees_Username ON Employees;
END
GO

-- Drop the Username column
IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Username' AND object_id = OBJECT_ID('Employees'))
BEGIN
    ALTER TABLE Employees DROP COLUMN Username;
END
GO

PRINT 'Username column removed successfully!';
GO