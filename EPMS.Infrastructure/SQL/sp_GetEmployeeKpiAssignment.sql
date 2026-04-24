CREATE PROCEDURE sp_GetEmployeeKpiAssignment
    @EmployeeId INT,
    @CycleId INT
AS
BEGIN
    SELECT 
        AssignmentId,
        EmployeeID,
        CycleID,
        KPI_ID,
        KpiNameSnapshot,
        CategorySnapshot,
        UnitSnapshot,
        Direction,
        WeightPercent,
        TargetValue,
        ActualValue,
        KpiScore,
        WeightedScore,
        Status,
        SUM(WeightPercent) OVER(PARTITION BY CategorySnapshot) AS CategoryWeightSubtotal
    FROM EmployeeKpiAssignment
    WHERE EmployeeID = @EmployeeId AND CycleID = @CycleId
    ORDER BY CategorySnapshot, KpiNameSnapshot;
END
GO
