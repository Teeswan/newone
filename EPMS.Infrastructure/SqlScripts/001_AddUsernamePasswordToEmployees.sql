-- Add Username and PasswordHash to Employees table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Username')
BEGIN
    ALTER TABLE Employees ADD Username NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(255) NULL;
END
GO

-- Add unique constraint for Username on Employees if not exists
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Employees_Username' AND object_id = OBJECT_ID('Employees'))
BEGIN
    CREATE UNIQUE INDEX UQ_Employees_Username ON Employees (Username) WHERE Username IS NOT NULL;
END
GO

-- Safer removal of columns from Users table (ignoring errors if dependencies exist)
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Username')
    BEGIN
        ALTER TABLE Users DROP COLUMN Username;
    END
END TRY
BEGIN CATCH
    -- Silently continue if column cannot be dropped due to constraints
END CATCH
GO

BEGIN TRY
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash')
    BEGIN
        ALTER TABLE Users DROP COLUMN PasswordHash;
    END
END TRY
BEGIN CATCH
    -- Silently continue
END CATCH
GO
