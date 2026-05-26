-- Fix Admin user's password hash
USE EmployeePerformance;
GO

-- First, let's get the password hasher to generate the correct hash!
-- Since we can't run C# here, let's use our existing DbInitializer logic - let's just update the Admin user's PasswordHash!
-- Wait, let's create a quick endpoint or let's manually update it using the same hasher!
-- Let's just write a SQL script that uses the same hashing algorithm (PBKDF2 with SHA256, 100000 iterations)!
-- But wait, let's instead just run a small C# snippet or let's use the DbInitializer to re-seed correctly!
-- Alternatively, let's just update the Admin user's PasswordHash to null, then restart the API so DbInitializer re-seeds it properly!
-- Let's do that!

UPDATE Employees 
SET PasswordHash = NULL 
WHERE EmployeeId = 1;

PRINT 'Admin user PasswordHash set to NULL - restart API to re-seed with proper hashed password!';
GO