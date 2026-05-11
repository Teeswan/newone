-- =============================================
-- Fix Admin Permissions Script
-- =============================================

USE [EmployeePerformance];
GO

PRINT 'Starting to fix admin permissions...';
GO

-- 1. Insert all missing permissions
PRINT 'Inserting missing permissions...';
GO

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Employees.View', 'View Employees');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Employees.Manage', 'Manage Employees');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Departments.View', 'View Departments');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Departments.Manage', 'Manage Departments');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Teams.View', 'View Teams');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Teams.Manage', 'Manage Teams');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Levels.View', 'View Levels');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Levels.Manage', 'Manage Levels');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Positions.View', 'View Positions');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Positions.Manage', 'Manage Positions');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.ViewPermissions')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Security.ViewPermissions', 'View Security Permissions');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Security.Manage', 'Manage Security Settings');

-- Appraisal permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalCycles.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalCycles.View', 'View Appraisal Cycles');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalCycles.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalCycles.Manage', 'Manage Appraisal Cycles');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalForms.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalForms.View', 'View Appraisal Forms');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalForms.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalForms.Manage', 'Manage Appraisal Forms');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalQuestions.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalQuestions.View', 'View Appraisal Questions');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalQuestions.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalQuestions.Manage', 'Manage Appraisal Questions');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalResponses.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalResponses.View', 'View Appraisal Responses');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalResponses.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.AppraisalResponses.Manage', 'Manage Appraisal Responses');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceEvaluations.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.PerformanceEvaluations.View', 'View Performance Evaluations');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceEvaluations.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.PerformanceEvaluations.Manage', 'Manage Performance Evaluations');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceOutcomes.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.PerformanceOutcomes.View', 'View Performance Outcomes');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceOutcomes.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.PerformanceOutcomes.Manage', 'Manage Performance Outcomes');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Meetings.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Meetings.View', 'View Meetings');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Meetings.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Meetings.Manage', 'Manage Meetings');

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Kpis.View')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Kpis.View', 'View KPIs');
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Kpis.Manage')
    INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Kpis.Manage', 'Manage KPIs');

PRINT 'Permissions inserted successfully!';
GO

-- 2. Ensure Admin position exists
PRINT 'Ensuring Admin position exists...';
GO

IF NOT EXISTS (SELECT 1 FROM Positions WHERE PositionTitle = 'Admin')
BEGIN
    INSERT INTO Positions (PositionTitle) VALUES ('Admin');
    PRINT 'Admin position created.';
END
ELSE
BEGIN
    PRINT 'Admin position already exists.';
END
GO

-- 3. Get Admin position ID
DECLARE @AdminPosId INT;
SELECT @AdminPosId = PositionId FROM Positions WHERE PositionTitle = 'Admin';

PRINT 'Admin Position ID: ' + CAST(@AdminPosId AS VARCHAR(10));
GO

-- 4. Assign ALL permissions to Admin position
PRINT 'Assigning all permissions to Admin position...';
GO

DECLARE @AdminId INT;
SELECT @AdminId = PositionId FROM Positions WHERE PositionTitle = 'Admin';

IF @AdminId IS NOT NULL
BEGIN
    INSERT INTO PositionPermissions (PositionId, PermissionId)
    SELECT @AdminId, PermissionId
    FROM Permissions p
    WHERE NOT EXISTS (
        SELECT 1 FROM PositionPermissions 
        WHERE PositionId = @AdminId AND PermissionId = p.PermissionId
    );
    
    PRINT 'Permissions assigned to Admin successfully!';
END
ELSE
BEGIN
    PRINT 'ERROR: Admin position not found!';
END
GO

-- 5. Ensure Admin employee exists
PRINT 'Ensuring Admin employee exists...';
GO

DECLARE @AdminPosId2 INT;
SELECT @AdminPosId2 = PositionId FROM Positions WHERE PositionTitle = 'Admin';

IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'EMP001')
BEGIN
    INSERT INTO Employees (EmployeeCode, FullName, PositionId, IsActive) 
    VALUES ('EMP001', 'System Admin', @AdminPosId2, 1);
    PRINT 'Admin employee created.';
END
ELSE
BEGIN
    -- Ensure employee has the correct position
    UPDATE Employees 
    SET PositionId = @AdminPosId2 
    WHERE EmployeeCode = 'EMP001';
    PRINT 'Admin employee already exists and position updated if needed.';
END
GO

-- 6. Ensure Admin user exists
PRINT 'Ensuring Admin user exists...';
GO

DECLARE @EmpId INT;
SELECT @EmpId = EmployeeId FROM Employees WHERE EmployeeCode = 'EMP001';

IF @EmpId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, EmployeeId) 
    VALUES ('admin', 'admin123', @EmpId);
    PRINT 'Admin user created.';
END
ELSE IF @EmpId IS NOT NULL
BEGIN
    PRINT 'Admin user already exists.';
END
ELSE
BEGIN
    PRINT 'ERROR: Admin employee not found!';
END
GO

-- 7. Show verification data
PRINT ' ';
PRINT '=============================================';
PRINT 'VERIFICATION DATA:';
PRINT '=============================================';
GO

-- Show Admin position
SELECT 'Admin Position' AS [Type], PositionId, PositionTitle 
FROM Positions 
WHERE PositionTitle = 'Admin';
GO

-- Show permissions assigned to Admin
DECLARE @AdminIdForVerify INT;
SELECT @AdminIdForVerify = PositionId FROM Positions WHERE PositionTitle = 'Admin';

SELECT 'Permissions for Admin' AS [Type], p.PermissionId, p.PermissionCode, p.Description
FROM Permissions p
INNER JOIN PositionPermissions pp ON p.PermissionId = pp.PermissionId
WHERE pp.PositionId = @AdminIdForVerify
ORDER BY p.PermissionCode;
GO

-- Show Admin employee
SELECT 'Admin Employee' AS [Type], EmployeeId, EmployeeCode, FullName, PositionId
FROM Employees WHERE EmployeeCode = 'EMP001';
GO

-- Show Admin user
SELECT 'Admin User' AS [Type], UserId, Username, EmployeeId
FROM Users WHERE Username = 'admin';
GO

PRINT ' ';
PRINT '=============================================';
PRINT 'Setup complete! Now log in with:';
PRINT '  Username: admin';
PRINT '  Password: admin123';
PRINT '=============================================';
GO
