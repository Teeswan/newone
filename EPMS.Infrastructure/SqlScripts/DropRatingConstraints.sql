-- Drop check constraints on PerformanceEvaluations that limit ratings to 0-5
-- This allows storing the total sum of ratings (Total Points)

DECLARE @ConstraintName nvarchar(200)

-- Find and drop constraint on ManagerRating
SELECT @ConstraintName = Name FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('PerformanceEvaluations')
AND definition LIKE '%ManagerRating%'

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE PerformanceEvaluations DROP CONSTRAINT ' + @ConstraintName)
    PRINT 'Dropped constraint: ' + @ConstraintName
END

-- Find and drop constraint on SelfRating
SET @ConstraintName = NULL
SELECT @ConstraintName = Name FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('PerformanceEvaluations')
AND definition LIKE '%SelfRating%'

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE PerformanceEvaluations DROP CONSTRAINT ' + @ConstraintName)
    PRINT 'Dropped constraint: ' + @ConstraintName
END
GO
