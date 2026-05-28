-- =============================================
-- Stored Procedures for PerformanceEvaluations table
-- =============================================

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetById
    @EvalID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations
    WHERE EvalID = @EvalID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetByEmployeeId
    @EmployeeID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations
    WHERE EmployeeID = @EmployeeID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetByCycleId
    @CycleID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations
    WHERE CycleID = @CycleID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_Create
    @EmployeeID INT = NULL,
    @CycleID INT = NULL,
    @FormID INT = NULL,
    @Status INT = 1,
    @SelfRating INT = NULL,
    @ManagerRating INT = NULL,
    @SelfComments NVARCHAR(MAX) = NULL,
    @ManagerComments NVARCHAR(MAX) = NULL,
    @CalibrationComments NVARCHAR(MAX) = NULL,
    @FinalRatingScore DECIMAL(5, 2) = NULL,
    @IsFinalized BIT = 0,
    @FinalizedAt DATETIME = NULL,
    @CreatedByEmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PerformanceEvaluations (EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
        SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, IsFinalized, FinalizedAt, CreatedByEmployeeId)
    VALUES (@EmployeeID, @CycleID, @FormID, @Status, @SelfRating, @ManagerRating,
        @SelfComments, @ManagerComments, @CalibrationComments, @FinalRatingScore, @IsFinalized, @FinalizedAt, @CreatedByEmployeeId);

    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations
    WHERE EvalID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_Update
    @EvalID INT,
    @EmployeeID INT = NULL,
    @CycleID INT = NULL,
    @FormID INT = NULL,
    @Status INT = NULL,
    @SelfRating INT = NULL,
    @ManagerRating INT = NULL,
    @SelfComments NVARCHAR(MAX) = NULL,
    @ManagerComments NVARCHAR(MAX) = NULL,
    @CalibrationComments NVARCHAR(MAX) = NULL,
    @FinalRatingScore DECIMAL(5, 2) = NULL,
    @IsFinalized BIT = NULL,
    @FinalizedAt DATETIME = NULL,
    @CreatedByEmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PerformanceEvaluations
    SET EmployeeID = ISNULL(@EmployeeID, EmployeeID),
        CycleID = ISNULL(@CycleID, CycleID),
        FormID = ISNULL(@FormID, FormID),
        Status = ISNULL(@Status, Status),
        SelfRating = ISNULL(@SelfRating, SelfRating),
        ManagerRating = ISNULL(@ManagerRating, ManagerRating),
        SelfComments = ISNULL(@SelfComments, SelfComments),
        ManagerComments = ISNULL(@ManagerComments, ManagerComments),
        CalibrationComments = ISNULL(@CalibrationComments, CalibrationComments),
        FinalRatingScore = ISNULL(@FinalRatingScore, FinalRatingScore),
        IsFinalized = ISNULL(@IsFinalized, IsFinalized),
        FinalizedAt = ISNULL(@FinalizedAt, FinalizedAt),
        CreatedByEmployeeId = ISNULL(@CreatedByEmployeeId, CreatedByEmployeeId)
    WHERE EvalID = @EvalID;

    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, CalibrationComments, FinalRatingScore, 
           IsFinalized, FinalizedAt, CreatedByEmployeeId
    FROM PerformanceEvaluations
    WHERE EvalID = @EvalID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_Delete
    @EvalID INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Delete related performance outcomes first
        DELETE FROM PerformanceOutcomes WHERE EvalID = @EvalID;
        
        -- Delete related appraisal responses
        DELETE FROM AppraisalResponses WHERE EvalID = @EvalID;
        
        -- Finally delete the evaluation itself
        DELETE FROM PerformanceEvaluations WHERE EvalID = @EvalID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
