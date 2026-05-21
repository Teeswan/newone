CREATE OR ALTER PROCEDURE sp_GetDepartmentKpiSummary
    @CycleId INT
AS
BEGIN
    SELECT 
        d.DepartmentID AS EntityId,
        d.DepartmentName AS EntityName,
        AVG(sub.TotalScore) AS AverageScore,
        COUNT(DISTINCT e.EmployeeID) AS EmployeeCount,
        @CycleId AS CycleId,
        (SELECT CycleName FROM AppraisalCycles WHERE CycleID = @CycleId) AS CycleName
    FROM Departments d
    JOIN Employees e ON d.DepartmentID = e.DepartmentID
    JOIN (
        SELECT EmployeeID, SUM(WeightedScore) AS TotalScore
        FROM EmployeeKpiAssignment
        WHERE CycleID = @CycleId
        GROUP BY EmployeeID
    ) sub ON e.EmployeeID = sub.EmployeeID
    WHERE d.IsDeleted = 0
    GROUP BY d.DepartmentID, d.DepartmentName;
END
GO
