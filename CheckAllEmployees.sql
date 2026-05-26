-- Check ALL employees
USE EmployeePerformance;
GO

SELECT 
    EmployeeId,
    EmployeeCode,
    FullName,
    Email,
    PasswordHash,
    IsFirstLogin,
    IsActive,
    IsDeleted
FROM Employees;
GO