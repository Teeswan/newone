-- Check Employee 2's details
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
FROM Employees
WHERE EmployeeId = 2;
GO