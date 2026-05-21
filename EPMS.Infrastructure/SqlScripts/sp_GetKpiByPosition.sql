USE [EmployeePerformance];
GO

CREATE OR ALTER PROCEDURE sp_GetKpiByPosition
    @PositionId INT = NULL,
    @IsActive BIT = 1, 
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SELECT 
        k.*,
        pk.PositionID,
        pk.DefaultWeightPercent,
        COUNT(*) OVER() AS TotalCount
    FROM KPIs k
    LEFT JOIN PositionKPIs pk ON k.KPI_ID = pk.KPI_ID
    WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)

      AND k.IsActive = @IsActive 
    ORDER BY k.KPIName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO