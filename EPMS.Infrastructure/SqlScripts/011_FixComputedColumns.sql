USE [EmployeePerformance];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT '============================================================';
PRINT 'Fixing Missing Computed Columns in EmployeeKpiAssignment';
PRINT '============================================================';
GO

-- 1. Drop existing columns to start fresh
IF COL_LENGTH('EmployeeKpiAssignment', 'WeightedScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN WeightedScore;
    PRINT '  ✓ Dropped existing WeightedScore';
END

IF COL_LENGTH('EmployeeKpiAssignment', 'KpiScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN KpiScore;
    PRINT '  ✓ Dropped existing KpiScore';
END

-- 2. Add KpiScore (Persisted)
PRINT '  Adding KpiScore...';
ALTER TABLE EmployeeKpiAssignment
ADD KpiScore AS 
(
    CASE 
        WHEN ActualValue IS NULL OR TargetValue = 0 THEN NULL
        WHEN Direction = 1 THEN 
            ROUND((ActualValue / TargetValue) * 100, 4)
        ELSE 
            CASE 
                WHEN ActualValue = 0 THEN 100
                WHEN (TargetValue / ActualValue) * 100 > 100 THEN 100
                ELSE ROUND((TargetValue / ActualValue) * 100, 4)
            END
    END
) PERSISTED;
PRINT '  ✓ KpiScore added';

-- 3. Add WeightedScore (Persisted)
PRINT '  Adding WeightedScore...';
ALTER TABLE EmployeeKpiAssignment
ADD WeightedScore AS 
(
    CASE 
        WHEN ActualValue IS NULL OR TargetValue = 0 THEN NULL
        ELSE 
            ROUND(
                (CASE 
                    WHEN Direction = 1 THEN 
                        ROUND((ActualValue / TargetValue) * 100, 4)
                    ELSE 
                        CASE 
                            WHEN ActualValue = 0 THEN 100
                            WHEN (TargetValue / ActualValue) * 100 > 100 THEN 100
                            ELSE ROUND((TargetValue / ActualValue) * 100, 4)
                        END
                END) * (WeightPercent / 100), 
            4)
    END
) PERSISTED;
PRINT '  ✓ WeightedScore added';

GO
PRINT '============================================================';
PRINT 'Computed Columns Fix Complete!';
PRINT '============================================================';
GO
