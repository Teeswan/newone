-- =============================================
-- Stored Procedures for ApplicationForms table
-- =============================================

CREATE OR ALTER PROCEDURE sp_ApplicationForms_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FormID, FormName, IsActive
    FROM ApplicationForms;
END
GO

CREATE OR ALTER PROCEDURE sp_ApplicationForms_GetById
    @FormID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FormID, FormName, IsActive
    FROM ApplicationForms
    WHERE FormID = @FormID;
END
GO

CREATE OR ALTER PROCEDURE sp_ApplicationForms_Create
    @FormName NVARCHAR(100) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ApplicationForms (FormName, IsActive)
    VALUES (@FormName, @IsActive);

    SELECT FormID, FormName, IsActive
    FROM ApplicationForms
    WHERE FormID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ApplicationForms_Update
    @FormID INT,
    @FormName NVARCHAR(100) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ApplicationForms
    SET FormName = @FormName,
        IsActive = @IsActive
    WHERE FormID = @FormID;

    SELECT FormID, FormName, IsActive
    FROM ApplicationForms
    WHERE FormID = @FormID;
END
GO

CREATE OR ALTER PROCEDURE sp_ApplicationForms_Delete
    @FormID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ApplicationForms WHERE FormID = @FormID;
END
GO
