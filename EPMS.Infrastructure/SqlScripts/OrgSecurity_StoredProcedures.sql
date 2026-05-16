-- =============================================
-- Author: EPMS Agent
-- Create date: 2026-04-22
-- Description: Stored Procedures for Org & Security Module
-- =============================================

-- 1. Get Department Tree (Recursive CTE)
CREATE OR ALTER PROCEDURE sp_GetDepartmentTree
AS
BEGIN
    SET NOCOUNT ON;

    WITH DeptTree AS (
        -- Anchor member: Root departments (those with no parent)
        SELECT 
            DepartmentId,
            DepartmentName,
            ParentDepartmentId,
            IsActive,
            0 AS Level
        FROM Departments
        WHERE ParentDepartmentId IS NULL AND IsActive = 1

        UNION ALL

        -- Recursive member: Child departments
        SELECT 
            d.DepartmentId,
            d.DepartmentName,
            d.ParentDepartmentId,
            d.IsActive,
            dt.Level + 1
        FROM Departments d
        INNER JOIN DeptTree dt ON d.ParentDepartmentId = dt.DepartmentId
        WHERE d.IsActive = 1
    )
    SELECT * FROM DeptTree
    ORDER BY Level, DepartmentName;
END
GO

-- 2. Get Permissions By Position
CREATE OR ALTER PROCEDURE sp_GetPermissionsByPosition
    @PositionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.PermissionId,
        p.PermissionCode,
        p.Description
    FROM Permissions p
    INNER JOIN PositionPermissions pp ON p.PermissionId = pp.PermissionId
    WHERE pp.PositionId = @PositionId;
END
GO

-- 3. Get Teams By Department
CREATE OR ALTER PROCEDURE sp_GetTeamsByDepartment
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.TeamId,
        t.TeamName,
        t.ManagerId,
        e.FullName AS ManagerName,
        t.DepartmentId,
        d.DepartmentName,
        (SELECT COUNT(*) FROM Employees WHERE TeamId = t.TeamId) AS MemberCount
    FROM Teams t
    LEFT JOIN Employees e ON t.ManagerId = e.EmployeeId
    LEFT JOIN Departments d ON t.DepartmentId = d.DepartmentId
    WHERE t.DepartmentId = @DepartmentId;
END
GO
