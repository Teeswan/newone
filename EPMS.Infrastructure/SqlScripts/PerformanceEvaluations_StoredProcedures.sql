-- =============================================
-- Stored Procedures for PerformanceEvaluations table
-- =============================================

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
    FROM PerformanceEvaluations;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_GetById
    @EvalID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
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
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
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
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
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
    @FinalRatingScore DECIMAL(5, 2) = NULL,
    @IsFinalized BIT = 0,
    @FinalizedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PerformanceEvaluations (EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
        SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt)
    VALUES (@EmployeeID, @CycleID, @FormID, @Status, @SelfRating, @ManagerRating,
        @SelfComments, @ManagerComments, @FinalRatingScore, @IsFinalized, @FinalizedAt);

    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
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
    @FinalRatingScore DECIMAL(5, 2) = NULL,
    @IsFinalized BIT = NULL,
    @FinalizedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PerformanceEvaluations
    SET EmployeeID = @EmployeeID,
        CycleID = @CycleID,
        FormID = @FormID,
        Status = ISNULL(@Status, Status),
        SelfRating = @SelfRating,
        ManagerRating = @ManagerRating,
        SelfComments = @SelfComments,
        ManagerComments = @ManagerComments,
        FinalRatingScore = @FinalRatingScore,
        IsFinalized = @IsFinalized,
        FinalizedAt = @FinalizedAt
    WHERE EvalID = @EvalID;

    SELECT EvalID, EmployeeID, CycleID, FormID, Status, SelfRating, ManagerRating,
           SelfComments, ManagerComments, FinalRatingScore, IsFinalized, FinalizedAt
    FROM PerformanceEvaluations
    WHERE EvalID = @EvalID;
END
GO

CREATE OR ALTER PROCEDURE sp_PerformanceEvaluations_Delete
    @EvalID INT
AS
BEGIN
    DELETE FROM PerformanceEvaluations WHERE EvalID = @EvalID;
END
GO
