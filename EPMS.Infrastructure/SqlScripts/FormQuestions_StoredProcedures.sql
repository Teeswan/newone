-- =============================================
-- Stored Procedures for FormQuestions table
-- =============================================

CREATE OR ALTER PROCEDURE sp_FormQuestions_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FormID, QuestionID, SortOrder
    FROM FormQuestions;
END
GO

CREATE OR ALTER PROCEDURE sp_FormQuestions_GetByFormId
    @FormID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FormID, QuestionID, SortOrder
    FROM FormQuestions
    WHERE FormID = @FormID
    ORDER BY SortOrder;
END
GO

CREATE OR ALTER PROCEDURE sp_FormQuestions_GetByFormAndQuestionId
    @FormID INT,
    @QuestionID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FormID, QuestionID, SortOrder
    FROM FormQuestions
    WHERE FormID = @FormID AND QuestionID = @QuestionID;
END
GO

CREATE OR ALTER PROCEDURE sp_FormQuestions_Create
    @FormID INT = NULL,
    @QuestionID INT = NULL,
    @SortOrder INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO FormQuestions (FormID, QuestionID, SortOrder)
    VALUES (@FormID, @QuestionID, @SortOrder);

    SELECT FormID, QuestionID, SortOrder
    FROM FormQuestions
    WHERE FormID = @FormID AND QuestionID = @QuestionID;
END
GO

CREATE OR ALTER PROCEDURE sp_FormQuestions_Update
    @FormID INT,
    @QuestionID INT,
    @SortOrder INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE FormQuestions
    SET SortOrder = @SortOrder
    WHERE FormID = @FormID AND QuestionID = @QuestionID;

    SELECT FormID, QuestionID, SortOrder
    FROM FormQuestions
    WHERE FormID = @FormID AND QuestionID = @QuestionID;
END
GO

CREATE OR ALTER PROCEDURE sp_FormQuestions_Delete
    @FormID INT,
    @QuestionID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM FormQuestions WHERE FormID = @FormID AND QuestionID = @QuestionID;
END
GO
