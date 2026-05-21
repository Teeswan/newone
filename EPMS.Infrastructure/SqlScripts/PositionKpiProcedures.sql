-- sp_PositionKpi_GetAll
CREATE OR ALTER PROCEDURE [dbo].[sp_PositionKpi_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        KpiId,
        KpiName,
        Category,
        Unit,
        WeightPercent,
        TargetValue,
        PriorityLevel,
        Direction,
        PositionId,
        IsActive,
        CreatedAt,
        CreatedByEmployeeId
    FROM dbo.KPIs 
    WHERE IsActive = 1;
END
GO

-- sp_PositionKpi_GetById
CREATE OR ALTER PROCEDURE [dbo].[sp_PositionKpi_GetById]
    @KpiId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        KpiId,
        KpiName,
        Category,
        Unit,
        WeightPercent,
        TargetValue,
        PriorityLevel,
        Direction,
        PositionId,
        IsActive,
        CreatedAt,
        CreatedByEmployeeId
    FROM dbo.KPIs 
    WHERE KpiId = @KpiId AND IsActive = 1;
END
GO

-- sp_PositionKpi_Create
CREATE OR ALTER PROCEDURE [dbo].[sp_PositionKpi_Create]
    @KpiName NVARCHAR(255),
    @Category NVARCHAR(100) = NULL,
    @Unit NVARCHAR(50) = NULL,
    @WeightPercent DECIMAL(5, 2),
    @TargetValue DECIMAL(18, 2) = NULL,
    @PriorityLevel NVARCHAR(20),
    @Direction INT,
    @PositionId INT = NULL,
    @CreatedByEmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.KPIs (
        KpiName, 
        Category, 
        Unit, 
        WeightPercent, 
        TargetValue, 
        PriorityLevel, 
        Direction, 
        PositionId, 
        IsActive, 
        CreatedAt, 
        CreatedByEmployeeId
    )
    VALUES (
        @KpiName, 
        @Category, 
        @Unit, 
        @WeightPercent, 
        @TargetValue, 
        @PriorityLevel, 
        @Direction, 
        @PositionId, 
        1, 
        GETUTCDATE(), 
        @CreatedByEmployeeId
    );
    
    DECLARE @NewKpiId INT = SCOPE_IDENTITY();
    SELECT * FROM dbo.KPIs WHERE KpiId = @NewKpiId;
END
GO

-- sp_PositionKpi_Update
CREATE OR ALTER PROCEDURE [dbo].[sp_PositionKpi_Update]
    @KpiId INT,
    @KpiName NVARCHAR(255),
    @Category NVARCHAR(100) = NULL,
    @Unit NVARCHAR(50) = NULL,
    @WeightPercent DECIMAL(5, 2),
    @TargetValue DECIMAL(18, 2) = NULL,
    @PriorityLevel NVARCHAR(20),
    @Direction INT,
    @PositionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.KPIs
    SET KpiName = @KpiName,
        Category = @Category,
        Unit = @Unit,
        WeightPercent = @WeightPercent,
        TargetValue = @TargetValue,
        PriorityLevel = @PriorityLevel,
        Direction = @Direction,
        PositionId = @PositionId
    WHERE KpiId = @KpiId AND IsActive = 1;

    SELECT * FROM dbo.KPIs WHERE KpiId = @KpiId;
END
GO

-- sp_PositionKpi_Delete
CREATE OR ALTER PROCEDURE [dbo].[sp_PositionKpi_Delete]
    @KpiId INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Soft delete by setting IsActive to 0
    UPDATE dbo.KPIs
    SET IsActive = 0
    WHERE KpiId = @KpiId;
END
GO
