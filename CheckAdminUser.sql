-- Check Admin user in Employees table
USE EmployeePerformance;
GO

SELECT EmployeeId, EmployeeCode, FullName, Email, PasswordHash, IsFirstLogin, IsActive, IsDeleted
FROM Employees
WHERE EmployeeCode = 'EMP001' OR Email LIKE '%admin%';
GO