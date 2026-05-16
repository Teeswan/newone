-- =============================================
-- Author: EPMS Agent
-- Create date: 2026-04-22
-- Description: Stored Procedures for Employee & Personnel Module
-- =============================================

-- 1. Get Employee Details (with Dept, Position, Manager)
CREATE OR ALTER PROCEDURE sp_GetEmployeeDetails
    @EmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.*,
        d.DepartmentName,
        p.PositionTitle,
        m.FullName AS ManagerName,
        l.LevelName
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
    LEFT JOIN Positions p ON e.PositionId = p.PositionId
    LEFT JOIN Levels l ON p.LevelId = l.LevelId
    LEFT JOIN Employees m ON e.ReportsTo = m.EmployeeId
    WHERE (@EmployeeId IS NULL OR e.EmployeeId = @EmployeeId)
    AND e.IsActive = 1;
END
GO

-- 2. Get Organization Hierarchy (Recursive CTE)
CREATE OR ALTER PROCEDURE sp_GetOrganizationHierarchy
    @ManagerId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    WITH OrgHierarchy AS (
        -- Anchor member: Starting manager or top-level managers
        SELECT 
            EmployeeId,
            FullName,
            ReportsTo,
            PositionId,
            DepartmentId,
            0 AS Level
        FROM Employees
        WHERE ((@ManagerId IS NULL AND ReportsTo IS NULL)
           OR (@ManagerId IS NOT NULL AND EmployeeId = @ManagerId))
        AND IsActive = 1

        UNION ALL

        -- Recursive member: Direct reports
        SELECT 
            e.EmployeeId,
            e.FullName,
            e.ReportsTo,
            e.PositionId,
            e.DepartmentId,
            oh.Level + 1
        FROM Employees e
        INNER JOIN OrgHierarchy oh ON e.ReportsTo = oh.EmployeeId
        WHERE e.IsActive = 1
    )
    SELECT 
        oh.*,
        p.PositionTitle,
        d.DepartmentName
    FROM OrgHierarchy oh
    LEFT JOIN Positions p ON oh.PositionId = p.PositionId
    LEFT JOIN Departments d ON oh.DepartmentId = d.DepartmentId
    ORDER BY Level, FullName;
END
GO
