CREATE OR ALTER PROCEDURE sp_GetPositionKpiSummary
    @CycleId INT
AS
BEGIN
    SELECT 
        p.PositionID AS EntityId,
        p.PositionName AS EntityName,
        AVG(sub.TotalScore) AS AverageScore,
        COUNT(DISTINCT e.EmployeeID) AS EmployeeCount,
        @CycleId AS CycleId,
        (SELECT CycleName FROM AppraisalCycles WHERE CycleID = @CycleId) AS CycleName
    FROM Positions p
    JOIN Employees e ON p.PositionID = e.PositionID
    JOIN (
        SELECT EmployeeID, SUM(WeightedScore) AS TotalScore
        FROM EmployeeKpiAssignment
        WHERE CycleID = @CycleId
        GROUP BY EmployeeID
    ) sub ON e.EmployeeID = sub.EmployeeID
    GROUP BY p.PositionID, p.PositionName;
END
GO
