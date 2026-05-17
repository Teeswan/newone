-- Fix Admin Login and IsDeleted initialization
USE [EmployeePerformance];
GO

-- 1. Ensure IsDeleted and IsActive columns have correct values for existing users
PRINT 'Initializing IsDeleted and IsActive for all employees...';
UPDATE Employees SET IsDeleted = 0 WHERE IsDeleted IS NULL;
UPDATE Employees SET IsActive = 1 WHERE IsActive IS NULL;

UPDATE Departments SET IsDeleted = 0 WHERE IsDeleted IS NULL;
UPDATE Departments SET IsActive = 1 WHERE IsActive IS NULL;

UPDATE Positions SET IsDeleted = 0 WHERE IsDeleted IS NULL;
UPDATE Positions SET IsActive = 1 WHERE IsActive IS NULL;

UPDATE Teams SET IsDeleted = 0 WHERE IsDeleted IS NULL;
UPDATE Teams SET IsActive = 1 WHERE IsActive IS NULL;
GO

-- 2. Reset Admin credentials and ensure not deleted
PRINT 'Resetting Admin credentials...';
DECLARE @AdminPosId INT = (SELECT PositionId FROM Positions WHERE PositionTitle = 'Admin');

IF @AdminPosId IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'EMP001')
    BEGIN
        UPDATE Employees 
        SET Username = 'admin', 
            PasswordHash = 'admin123',
            PositionId = @AdminPosId,
            IsDeleted = 0,
            IsActive = 1
        WHERE EmployeeCode = 'EMP001';
        PRINT '✅ Admin employee EMP001 updated.';
    END
    ELSE
    BEGIN
        INSERT INTO Employees (EmployeeCode, FullName, PositionId, IsActive, Username, PasswordHash, IsDeleted) 
        VALUES ('EMP001', 'System Admin', @AdminPosId, 1, 'admin', 'admin123', 0);
        PRINT '✅ Admin employee EMP001 created.';
    END
END
ELSE
BEGIN
    PRINT '❌ Admin position not found. Please run CompleteSetup.sql first.';
END
GO

PRINT 'Done!';
