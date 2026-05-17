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
-- Find Admin position ID (case insensitive search)
DECLARE @AdminPosId INT = (SELECT TOP 1 PositionId FROM Positions WHERE PositionTitle LIKE '%Admin%');

IF @AdminPosId IS NOT NULL
BEGIN
    -- Update existing admin by code OR username
    IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'EMP001' OR Username = 'admin')
    BEGIN
        UPDATE Employees 
        SET Username = 'admin', 
            PasswordHash = 'admin123',
            PositionId = @AdminPosId,
            IsDeleted = 0,
            IsActive = 1
        WHERE EmployeeCode = 'EMP001' OR Username = 'admin';
        PRINT '✅ Admin employee updated.';
    END
    ELSE
    BEGIN
        INSERT INTO Employees (EmployeeCode, FullName, PositionId, IsActive, Username, PasswordHash, IsDeleted) 
        VALUES ('EMP001', 'System Admin', @AdminPosId, 1, 'admin', 'admin123', 0);
        PRINT '✅ Admin employee created.';
    END
END
ELSE
BEGIN
    PRINT '❌ Admin position not found. Creating it...';
    INSERT INTO Positions (PositionTitle, IsActive, IsDeleted) VALUES ('Admin', 1, 0);
    SET @AdminPosId = SCOPE_IDENTITY();
    
    INSERT INTO Employees (EmployeeCode, FullName, PositionId, IsActive, Username, PasswordHash, IsDeleted) 
    VALUES ('EMP001', 'System Admin', @AdminPosId, 1, 'admin', 'admin123', 0);
    PRINT '✅ Admin position and employee created.';
END
GO

PRINT 'Done!';
