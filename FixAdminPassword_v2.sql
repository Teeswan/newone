-- Fix Admin user - delete and let DbInitializer recreate
USE EmployeePerformance;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- First delete the existing admin user so DbInitializer will recreate with proper hashed password
DELETE FROM Employees WHERE EmployeeId = 1;
GO

PRINT 'Admin user deleted - restart API to re-seed with proper hashed password!';
GO