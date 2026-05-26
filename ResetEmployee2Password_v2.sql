-- Reset Employee 2's password hash with correct SET options
USE EmployeePerformance;
GO

SET QUOTED_IDENTIFIER ON;
GO

UPDATE Employees 
SET PasswordHash = NULL 
WHERE EmployeeId = 2;

PRINT 'Employee 2 password reset - restart API to initialize with default password!';
GO