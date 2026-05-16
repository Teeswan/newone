-- =============================================
-- Stored Procedures for PerformanceOutcomes table
-- =============================================

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_GetById
    @OutcomeID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes
    WHERE OutcomeID = @OutcomeID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_GetByEmployeeId
    @EmployeeID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes
    WHERE EmployeeID = @EmployeeID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_GetByCycleId
    @CycleID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes
    WHERE CycleID = @CycleID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_Create
    @EvalID INT = NULL,
    @EmployeeID INT = NULL,
    @CycleID INT = NULL,
    @RecommendationType NVARCHAR(50) = NULL,
    @OldPositionID INT = NULL,
    @NewPositionID INT = NULL,
    @OldLevelID NVARCHAR(10) = NULL,
    @NewLevelID NVARCHAR(10) = NULL,
    @ApprovalStatus NVARCHAR(20) = 'Pending',
    @EffectiveDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PerformanceOutcomes (EvalID, EmployeeID, CycleID, RecommendationType,
        OldPositionID, NewPositionID, OldLevelID, NewLevelID, ApprovalStatus, EffectiveDate)
    VALUES (@EvalID, @EmployeeID, @CycleID, @RecommendationType,
        @OldPositionID, @NewPositionID, @OldLevelID, @NewLevelID, @ApprovalStatus, @EffectiveDate);

    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes
    WHERE OutcomeID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_Update
    @OutcomeID INT,
    @EvalID INT = NULL,
    @EmployeeID INT = NULL,
    @CycleID INT = NULL,
    @RecommendationType NVARCHAR(50) = NULL,
    @OldPositionID INT = NULL,
    @NewPositionID INT = NULL,
    @OldLevelID NVARCHAR(10) = NULL,
    @NewLevelID NVARCHAR(10) = NULL,
    @ApprovalStatus NVARCHAR(20) = NULL,
    @EffectiveDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PerformanceOutcomes
    SET EvalID = @EvalID,
        EmployeeID = @EmployeeID,
        CycleID = @CycleID,
        RecommendationType = @RecommendationType,
        OldPositionID = @OldPositionID,
        NewPositionID = @NewPositionID,
        OldLevelID = @OldLevelID,
        NewLevelID = @NewLevelID,
        ApprovalStatus = @ApprovalStatus,
        EffectiveDate = @EffectiveDate
    WHERE OutcomeID = @OutcomeID;

    SELECT OutcomeID, EvalID, EmployeeID, CycleID, RecommendationType,
           OldPositionID, NewPositionID, OldLevelID, NewLevelID,
           ApprovalStatus, EffectiveDate
    FROM PerformanceOutcomes
    WHERE OutcomeID = @OutcomeID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceOutcomes_Delete
    @OutcomeID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PerformanceOutcomes WHERE OutcomeID = @OutcomeID;
END
GO
