USE [EmployeePerformance];
GO

PRINT '============================================================';
PRINT 'Cleaning up KPIs Table - Removing Extra Columns';
PRINT '============================================================';
GO

-- ============================================================
-- Helper: Drop default constraint for a column
-- ============================================================
DECLARE @sql NVARCHAR(MAX);
DECLARE @columnName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = 'KPIs';

-- ============================================================
-- Step 1: Remove extra columns from KPIs table
-- ============================================================
PRINT '';
PRINT 'Step 1: Removing extra columns from KPIs table...';

-- ------------------------------------------------------------
-- 1. WeightPercent
-- ------------------------------------------------------------
SET @columnName = 'WeightPercent';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ------------------------------------------------------------
-- 2. TargetValue
-- ------------------------------------------------------------
SET @columnName = 'TargetValue';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ------------------------------------------------------------
-- 3. PriorityLevel
-- ------------------------------------------------------------
SET @columnName = 'PriorityLevel';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ------------------------------------------------------------
-- 4. PositionId
-- ------------------------------------------------------------
SET @columnName = 'PositionId';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ------------------------------------------------------------
-- 5. CreatedAt
-- ------------------------------------------------------------
SET @columnName = 'CreatedAt';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ------------------------------------------------------------
-- 6. CreatedByEmployeeId
-- ------------------------------------------------------------
SET @columnName = 'CreatedByEmployeeId';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName)
BEGIN
    -- First drop any default constraint
    SELECT @sql = 'ALTER TABLE ' + @tableName + ' DROP CONSTRAINT [' + d.name + ']'
    FROM sys.default_constraints d
    WHERE d.parent_object_id = OBJECT_ID(@tableName)
      AND d.parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = @columnName);
    
    IF @sql IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Dropped default constraint for ' + @columnName;
        SET @sql = NULL;
    END
    
    EXEC('ALTER TABLE ' + @tableName + ' DROP COLUMN ' + @columnName);
    PRINT '  ✓ Removed ' + @columnName + ' from ' + @tableName;
END
ELSE
BEGIN
    PRINT '  ✓ ' + @columnName + ' already removed from ' + @tableName;
END

-- ============================================================
-- Step 2: Verify the cleanup
-- ============================================================
PRINT '';
PRINT 'Step 2: Verifying KPIs table structure...';
PRINT '';

SELECT 
    COLUMN_NAME AS ColumnName,
    DATA_TYPE AS DataType
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'KPIs'
ORDER BY ORDINAL_POSITION;

-- ============================================================
-- Complete
-- ============================================================
PRINT '';
PRINT '============================================================';
PRINT 'KPIs Table Cleanup Complete!';
PRINT '============================================================';
PRINT '';
PRINT 'The KPIs table now only contains:';
PRINT '  - KpiID (Primary Key)';
PRINT '  - KPIName';
PRINT '  - Category';
PRINT '  - Unit';
PRINT '  - IsActive';
PRINT '  - Direction';
PRINT '';
PRINT 'The PositionKPIs table contains:';
PRINT '  - PositionKPIID (Primary Key)';
PRINT '  - PositionID';
PRINT '  - KpiID (Foreign Key to KPIs)';
PRINT '  - DefaultWeightPercent';
PRINT '  - IsRequired';
GO
