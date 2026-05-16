-- Step 1: Add Username and PasswordHash columns to Employees table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Username')
BEGIN
    ALTER TABLE Employees ADD Username NVARCHAR(50) NULL;
    PRINT '✅ Added Username column to Employees table';
END
ELSE
BEGIN
    PRINT 'ℹ️ Username column already exists in Employees table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(255) NULL;
    PRINT '✅ Added PasswordHash column to Employees table';
END
ELSE
BEGIN
    PRINT 'ℹ️ PasswordHash column already exists in Employees table';
END
GO

-- Step 2: Add unique constraint for Username (if not exists)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Employees_Username' AND object_id = OBJECT_ID('Employees'))
BEGIN
    CREATE UNIQUE INDEX UQ_Employees_Username ON Employees (Username) WHERE Username IS NOT NULL;
    PRINT '✅ Added unique index for Username on Employees table';
END
ELSE
BEGIN
    PRINT 'ℹ️ Unique index for Username already exists';
END
GO

-- Step 3: Make sure Admin position exists
IF NOT EXISTS (SELECT 1 FROM Positions WHERE PositionTitle = 'Admin')
BEGIN
    INSERT INTO Positions (PositionTitle) VALUES ('Admin');
    PRINT '✅ Added Admin position';
END
ELSE
BEGIN
    PRINT 'ℹ️ Admin position already exists';
END
GO

-- Step 4: Make sure all permissions are seeded
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
PRINT '✅ All permissions checked';
GO

-- Step 5: Assign all permissions to Admin position
DECLARE @AdminPosId INT = (SELECT PositionId FROM Positions WHERE PositionTitle = 'Admin');
IF @AdminPosId IS NOT NULL
BEGIN
    INSERT INTO PositionPermissions (PositionId, PermissionId)
    SELECT @AdminPosId, PermissionId
    FROM Permissions p
    WHERE NOT EXISTS (SELECT 1 FROM PositionPermissions WHERE PositionId = @AdminPosId AND PermissionId = p.PermissionId);
    PRINT '✅ Admin position permissions assigned';
END
GO

-- Step 6: Create or update admin employee
DECLARE @AdminPosId INT = (SELECT PositionId FROM Positions WHERE PositionTitle = 'Admin');
IF @AdminPosId IS NOT NULL
BEGIN
    -- Check if admin employee exists
    IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'EMP001')
    BEGIN
        INSERT INTO Employees (EmployeeCode, FullName, PositionId, IsActive, Username, PasswordHash) 
        VALUES ('EMP001', 'System Admin', @AdminPosId, 1, 'admin', 'admin123');
        PRINT '✅ Admin employee created';
    END
    ELSE
    BEGIN
        -- Update existing admin employee with username and password
        UPDATE Employees 
        SET Username = 'admin', 
            PasswordHash = 'admin123',
            PositionId = @AdminPosId
        WHERE EmployeeCode = 'EMP001';
        PRINT '✅ Admin employee updated with login credentials';
    END
END
GO

PRINT '';
PRINT '🎉 Complete! Now you can login with:';
PRINT '   Username: admin';
PRINT '   Password: admin123';