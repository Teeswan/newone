-- Migration: Add computed columns for KpiScore and WeightedScore
-- Date: 2026-05-26

-- First, drop the existing columns if they exist
IF COL_LENGTH('EmployeeKpiAssignment', 'KpiScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN KpiScore;
END

IF COL_LENGTH('EmployeeKpiAssignment', 'WeightedScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN WeightedScore;
END

-- Add KpiScore as a PERSISTED computed column
ALTER TABLE EmployeeKpiAssignment
ADD KpiScore AS 
(
    CASE 
        WHEN ActualValue IS NULL OR TargetValue = 0 THEN NULL
        WHEN Direction = 1 THEN 
            ROUND((ActualValue / TargetValue) * 100, 2)
        ELSE 
            CASE 
                WHEN ActualValue = 0 THEN 100
                WHEN (TargetValue / ActualValue) * 100 > 100 THEN 100
                ELSE ROUND((TargetValue / ActualValue) * 100, 2)
            END
    END
) PERSISTED;

-- Add WeightedScore as a PERSISTED computed column
ALTER TABLE EmployeeKpiAssignment
ADD WeightedScore AS 
(
    CASE 
        WHEN KpiScore IS NULL THEN NULL
        ELSE ROUND(KpiScore * (WeightPercent / 100), 2)
    END
) PERSISTED;

PRINT 'Computed columns KpiScore and WeightedScore added successfully!';
