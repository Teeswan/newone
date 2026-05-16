-- Add EvalID to PerformanceOutcomes if it does not already exist
IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PerformanceOutcomes')
      AND name = 'EvalID'
)
BEGIN
    ALTER TABLE dbo.PerformanceOutcomes
    ADD EvalID INT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID(N'dbo.PerformanceOutcomes')
      AND name = 'FK_PerformanceOutcome_PerformanceEvaluation'
)
BEGIN
    ALTER TABLE dbo.PerformanceOutcomes
    ADD CONSTRAINT FK_PerformanceOutcome_PerformanceEvaluation
        FOREIGN KEY (EvalID)
        REFERENCES dbo.PerformanceEvaluations (EvalID);
END;
