CREATE OR ALTER PROCEDURE sp_GetTeamKpiSummary
    @CycleId INT
AS
BEGIN
    SELECT 
        t.TeamID AS EntityId,
        t.TeamName AS EntityName,
        AVG(sub.TotalScore) AS AverageScore,
        COUNT(DISTINCT tm.EmployeeID) AS EmployeeCount,
        @CycleId AS CycleId,
        (SELECT CycleName FROM AppraisalCycles WHERE CycleID = @CycleId) AS CycleName
    FROM Teams t
    JOIN TeamMembers tm ON t.TeamID = tm.TeamID
    JOIN (
        SELECT EmployeeID, SUM(WeightedScore) AS TotalScore
        FROM EmployeeKpiAssignment
        WHERE CycleID = @CycleId
        GROUP BY EmployeeID
    ) sub ON tm.EmployeeID = sub.EmployeeID
    WHERE t.IsDeleted = 0
    GROUP BY t.TeamID, t.TeamName;
END
