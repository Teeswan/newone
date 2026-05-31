-- Add IsActive and LevelDescription to Levels table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Levels') AND name = 'LevelDescription')
BEGIN
    ALTER TABLE Levels ADD LevelDescription NVARCHAR(500) NULL;
    PRINT '✅ Added LevelDescription column to Levels table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Levels') AND name = 'IsActive')
BEGIN
    ALTER TABLE Levels ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT '✅ Added IsActive column to Levels table';
END
GO
