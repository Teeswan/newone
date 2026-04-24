CREATE PROCEDURE sp_GetKpisByPosition
    @PositionId INT = NULL,
    @IsActive BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SELECT 
        *,
        COUNT(*) OVER() AS TotalCount
    FROM KPIs
    WHERE (PositionId = @PositionId OR @PositionId IS NULL)
      AND IsActive = @IsActive
    ORDER BY KPIName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
