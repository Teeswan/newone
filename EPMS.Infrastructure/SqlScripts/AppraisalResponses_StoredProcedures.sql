-- =============================================
-- Stored Procedures for AppraisalResponses table
-- =============================================

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ResponseID, EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous
    FROM AppraisalResponses;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_GetById
    @ResponseID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ResponseID, EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous
    FROM AppraisalResponses
    WHERE ResponseID = @ResponseID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_GetByEvalId
    @EvalID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ResponseID, EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous
    FROM AppraisalResponses
    WHERE EvalID = @EvalID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_Create
    @EvalID INT = NULL,
    @QuestionID INT = NULL,
    @RespondentID INT = NULL,
    @RespondentEmployeeID INT = NULL,
    @RespondentRole NVARCHAR(100) = NULL,
    @AnswerText NVARCHAR(MAX) = NULL,
    @RatingValue INT = NULL,
    @IsAnonymous BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AppraisalResponses (EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous)
    VALUES (@EvalID, @QuestionID, @RespondentID, @RespondentEmployeeID, @RespondentRole, @AnswerText, @RatingValue, ISNULL(@IsAnonymous, 0));

    SELECT ResponseID, EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous
    FROM AppraisalResponses
    WHERE ResponseID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_Update
    @ResponseID BIGINT,
    @EvalID INT = NULL,
    @QuestionID INT = NULL,
    @RespondentID INT = NULL,
    @RespondentEmployeeID INT = NULL,
    @RespondentRole NVARCHAR(100) = NULL,
    @AnswerText NVARCHAR(MAX) = NULL,
    @RatingValue INT = NULL,
    @IsAnonymous BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AppraisalResponses
    SET EvalID = @EvalID,
        QuestionID = @QuestionID,
        RespondentID = @RespondentID,
        RespondentEmployeeID = @RespondentEmployeeID,
        RespondentRole = @RespondentRole,
        AnswerText = @AnswerText,
        RatingValue = @RatingValue,
        IsAnonymous = ISNULL(@IsAnonymous, IsAnonymous)
    WHERE ResponseID = @ResponseID;

    SELECT ResponseID, EvalID, QuestionID, RespondentID, RespondentEmployeeID, RespondentRole, AnswerText, RatingValue, IsAnonymous
    FROM AppraisalResponses
    WHERE ResponseID = @ResponseID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalResponses_Delete
    @ResponseID BIGINT
AS
BEGIN
    DELETE FROM AppraisalResponses WHERE ResponseID = @ResponseID;
END
GO
