-- Migration: Update KPI-related columns to use explicit DECIMAL types
-- Date: 2026-05-26

-- ============================================
-- TeamKPIs Table
-- ============================================

IF COL_LENGTH('TeamKPIs', 'TeamTarget') IS NOT NULL
BEGIN
    ALTER TABLE TeamKPIs 
    ALTER COLUMN TeamTarget DECIMAL(18, 4) NOT NULL;
END

IF COL_LENGTH('TeamKPIs', 'Weight') IS NOT NULL
BEGIN
    ALTER TABLE TeamKPIs 
    ALTER COLUMN Weight DECIMAL(5, 2) NOT NULL;
END

-- ============================================
-- DepartmentKPIs Table
-- ============================================

IF COL_LENGTH('DepartmentKPIs', 'DepartmentTarget') IS NOT NULL
BEGIN
    ALTER TABLE DepartmentKPIs 
    ALTER COLUMN DepartmentTarget DECIMAL(18, 4) NOT NULL;
END

IF COL_LENGTH('DepartmentKPIs', 'Weight') IS NOT NULL
BEGIN
    ALTER TABLE DepartmentKPIs 
    ALTER COLUMN Weight DECIMAL(5, 2) NOT NULL;
END

-- ============================================
-- EmployeeKPIs Table
-- ============================================

IF COL_LENGTH('EmployeeKPIs', 'TargetValue') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKPIs 
    ALTER COLUMN TargetValue DECIMAL(18, 4) NOT NULL;
END

IF COL_LENGTH('EmployeeKPIs', 'WeightPercent') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKPIs 
    ALTER COLUMN WeightPercent DECIMAL(5, 2) NOT NULL;
END

-- ============================================
-- EmployeeKpiAssignment Table
-- ============================================
-- Note: If you already ran 003_AddComputedKpiScores.sql,
-- you'll need to drop and recreate the computed columns first

-- First, check if computed columns exist and drop them
IF COL_LENGTH('EmployeeKpiAssignment', 'KpiScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN KpiScore;
END

IF COL_LENGTH('EmployeeKpiAssignment', 'WeightedScore') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment DROP COLUMN WeightedScore;
END

-- Now alter the base columns
IF COL_LENGTH('EmployeeKpiAssignment', 'TargetValue') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment 
    ALTER COLUMN TargetValue DECIMAL(18, 4) NOT NULL;
END

IF COL_LENGTH('EmployeeKpiAssignment', 'ActualValue') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment 
    ALTER COLUMN ActualValue DECIMAL(18, 4);
END

IF COL_LENGTH('EmployeeKpiAssignment', 'WeightPercent') IS NOT NULL
BEGIN
    ALTER TABLE EmployeeKpiAssignment 
    ALTER COLUMN WeightPercent DECIMAL(5, 2) NOT NULL;
END

-- Re-add the computed columns with updated precision
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

ALTER TABLE EmployeeKpiAssignment
ADD WeightedScore AS 
(
    CASE 
        WHEN KpiScore IS NULL THEN NULL
        ELSE ROUND(KpiScore * (WeightPercent / 100), 4)
    END
) PERSISTED;

PRINT 'Successfully updated all KPI-related columns to use explicit DECIMAL types!';
