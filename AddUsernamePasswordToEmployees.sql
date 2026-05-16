-- Add Username and PasswordHash to Employees table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Username')
BEGIN
    ALTER TABLE Employees ADD Username NVARCHAR(50) NULL;
    PRINT 'Added Username column to Employees table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(255) NULL;
    PRINT 'Added PasswordHash column to Employees table';
END
GO

-- Add unique constraint for Username on Employees
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Employees_Username' AND object_id = OBJECT_ID('Employees'))
BEGIN
    CREATE UNIQUE INDEX UQ_Employees_Username ON Employees (Username) WHERE Username IS NOT NULL;
    PRINT 'Added unique index for Username on Employees table';
END
GO

-- Remove Username and PasswordHash from Users table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Username')
BEGIN
    ALTER TABLE Users DROP CONSTRAINT IF EXISTS UQ__Users__536C85E4BCAAD821;
    ALTER TABLE Users DROP COLUMN Username;
    PRINT 'Removed Username column from Users table';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Users DROP COLUMN PasswordHash;
    PRINT 'Removed PasswordHash column from Users table';
END
GO

PRINT 'Migration completed successfully!';