-- =============================================
-- Quick Seed Permissions Script
-- Run this in SSMS!
-- =============================================

USE [EmployeePerformance];
GO

PRINT '--- Quick Seed ---';
GO

-- 1. Insert all permissions
PRINT 'Inserting permissions...';
GO

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Employees.View', 'View Employees' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Employees.Manage', 'Manage Employees' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.Manage');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Employees.TeamEmployeeManagement', 'Team-Scoped Employee Management' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.TeamEmployeeManagement');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.DepartmentScopedManagement', 'Department-Scoped Management (teams + read-only employees in department)' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.DepartmentScopedManagement');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.GlobalAdminManagement', 'Global Admin Management (full organization hierarchy access)' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.GlobalAdminManagement');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Departments.View', 'View Departments' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Departments.Manage', 'Manage Departments' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Teams.View', 'View Teams' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Teams.Manage', 'Manage Teams' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Levels.View', 'View Levels' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Levels.Manage', 'Manage Levels' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Positions.View', 'View Positions' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Positions.Manage', 'Manage Positions' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Security.ViewPermissions', 'View Security Permissions' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.ViewPermissions');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Security.Manage', 'Manage Security Settings' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalCycles.View', 'View Appraisal Cycles' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalCycles.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalCycles.Manage', 'Manage Appraisal Cycles' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalCycles.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalForms.View', 'View Appraisal Forms' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalForms.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalForms.Manage', 'Manage Appraisal Forms' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalForms.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalQuestions.View', 'View Appraisal Questions' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalQuestions.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalQuestions.Manage', 'Manage Appraisal Questions' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalQuestions.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalResponses.View', 'View Appraisal Responses' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalResponses.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.AppraisalResponses.Manage', 'Manage Appraisal Responses' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.AppraisalResponses.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.PerformanceEvaluations.View', 'View Performance Evaluations' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceEvaluations.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.PerformanceEvaluations.Manage', 'Manage Performance Evaluations' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceEvaluations.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.PerformanceOutcomes.View', 'View Performance Outcomes' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceOutcomes.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.PerformanceOutcomes.Manage', 'Manage Performance Outcomes' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.PerformanceOutcomes.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Meetings.View', 'View Meetings' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Meetings.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Meetings.Manage', 'Manage Meetings' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Meetings.Manage');

INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Kpis.View', 'View KPIs' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Kpis.View');
INSERT INTO Permissions (PermissionCode, Description) 
SELECT 'Permissions.Kpis.Manage', 'Manage KPIs' WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Kpis.Manage');

PRINT 'Permissions inserted!';
GO

-- 2. Verify data
PRINT '--- Data in database ---';
SELECT COUNT(*) as PermissionCount FROM Permissions;
SELECT * FROM Permissions ORDER BY PermissionId;
GO

PRINT 'Done! Now refresh the Blazor page and try assigning permissions!';
GO
