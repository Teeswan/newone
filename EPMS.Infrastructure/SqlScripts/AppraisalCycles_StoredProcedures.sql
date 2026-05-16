-- =============================================
-- Stored Procedures for AppraisalCycles table
-- =============================================

CREATE OR ALTER PROCEDURE sp_AppraisalCycles_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CycleID, CycleName, StartDate, EndDate, EvaluationPeriod, CycleStatus
    FROM AppraisalCycles;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalCycles_GetById
    @CycleID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CycleID, CycleName, StartDate, EndDate, EvaluationPeriod, CycleStatus
    FROM AppraisalCycles
    WHERE CycleID = @CycleID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalCycles_Create
    @CycleName NVARCHAR(100),
    @StartDate DATE,
    @EndDate DATE,
    @EvaluationPeriod NVARCHAR(50) = NULL,
    @CycleStatus NVARCHAR(20) = 'Draft'
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AppraisalCycles (CycleName, StartDate, EndDate, EvaluationPeriod, CycleStatus)
    VALUES (@CycleName, @StartDate, @EndDate, @EvaluationPeriod, @CycleStatus);

    SELECT CycleID, CycleName, StartDate, EndDate, EvaluationPeriod, CycleStatus
    FROM AppraisalCycles
    WHERE CycleID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalCycles_Update
    @CycleID INT,
    @CycleName NVARCHAR(100),
    @StartDate DATE,
    @EndDate DATE,
    @EvaluationPeriod NVARCHAR(50) = NULL,
    @CycleStatus NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AppraisalCycles
    SET CycleName = @CycleName,
        StartDate = @StartDate,
        EndDate = @EndDate,
        EvaluationPeriod = @EvaluationPeriod,
        CycleStatus = @CycleStatus
    WHERE CycleID = @CycleID;

    SELECT CycleID, CycleName, StartDate, EndDate, EvaluationPeriod, CycleStatus
    FROM AppraisalCycles
    WHERE CycleID = @CycleID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalCycles_Delete
    @CycleID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM AppraisalCycles WHERE CycleID = @CycleID;
END
GO
