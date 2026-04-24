CREATE PROCEDURE sp_GetKpiScoreSummary
    @EmployeeId INT,
    @CycleId INT
AS
BEGIN
    WITH ScoreData AS (
        SELECT 
            SUM(WeightedScore) AS TotalPerformanceScore,
            COUNT(*) AS KpiCount,
            AVG(KpiScore) AS AverageKpiScore
        FROM EmployeeKpiAssignment
        WHERE EmployeeID = @EmployeeId AND CycleID = @CycleId
    )
    SELECT 
        TotalPerformanceScore,
        KpiCount,
        AverageKpiScore,
        CASE 
            WHEN TotalPerformanceScore >= 86 THEN 5 
            WHEN TotalPerformanceScore >= 71 THEN 4 
            WHEN TotalPerformanceScore >= 60 THEN 3 
            WHEN TotalPerformanceScore >= 40 THEN 2 
            ELSE 1 
        END AS Rating
    FROM ScoreData;
END
GO
