-- =============================================
-- Stored Procedures for AppraisalQuestions table
-- =============================================

CREATE OR ALTER PROCEDURE sp_AppraisalQuestions_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT QuestionID, QuestionText, Category, IsRequired
    FROM AppraisalQuestions;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalQuestions_GetById
    @QuestionID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT QuestionID, QuestionText, Category, IsRequired
    FROM AppraisalQuestions
    WHERE QuestionID = @QuestionID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalQuestions_Create
    @QuestionText NVARCHAR(MAX),
    @Category NVARCHAR(100) = NULL,
    @IsRequired BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AppraisalQuestions (QuestionText, Category, IsRequired)
    VALUES (@QuestionText, @Category, @IsRequired);

    SELECT QuestionID, QuestionText, Category, IsRequired
    FROM AppraisalQuestions
    WHERE QuestionID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalQuestions_Update
    @QuestionID INT,
    @QuestionText NVARCHAR(MAX),
    @Category NVARCHAR(100) = NULL,
    @IsRequired BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AppraisalQuestions
    SET QuestionText = @QuestionText,
        Category = @Category,
        IsRequired = @IsRequired
    WHERE QuestionID = @QuestionID;

    SELECT QuestionID, QuestionText, Category, IsRequired
    FROM AppraisalQuestions
    WHERE QuestionID = @QuestionID;
END
GO

CREATE OR ALTER PROCEDURE sp_AppraisalQuestions_Delete
    @QuestionID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM AppraisalQuestions WHERE QuestionID = @QuestionID;
END
GO
