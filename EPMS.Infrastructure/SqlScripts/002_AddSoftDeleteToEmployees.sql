-- Add IsDeleted to Employees table for soft delete functionality
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE Employees ADD IsDeleted BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsDeleted column to Employees table';
END
GO
