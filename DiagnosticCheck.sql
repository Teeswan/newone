-- Diagnostic script to verify login and permissions setup
PRINT '============================================';
PRINT '  EPMS Setup Diagnostics';
PRINT '============================================';
PRINT '';

-- 1. Check Employees table has Username and PasswordHash
PRINT '1. Checking Employees table columns:';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Username')
    PRINT '   ✅ Username column exists';
ELSE
    PRINT '   ❌ Username column MISSING!';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
    PRINT '   ✅ PasswordHash column exists';
ELSE
    PRINT '   ❌ PasswordHash column MISSING!';
PRINT '';

-- 2. Check Admin position
PRINT '2. Checking Admin position:';
DECLARE @AdminPosId INT;
SELECT @AdminPosId = PositionId FROM Positions WHERE PositionTitle = 'Admin';
IF @AdminPosId IS NOT NULL
    PRINT '   ✅ Admin position found (PositionId: ' + CAST(@AdminPosId AS VARCHAR) + ')';
ELSE
    PRINT '   ❌ Admin position NOT FOUND!';
PRINT '';

-- 3. Check Admin employee
PRINT '3. Checking Admin employee (EMP001):';
DECLARE @AdminEmpId INT, @AdminEmpPosId INT, @AdminUsername NVARCHAR(50), @AdminPassword NVARCHAR(255);
SELECT 
    @AdminEmpId = EmployeeId,
    @AdminEmpPosId = PositionId,
    @AdminUsername = Username,
    @AdminPassword = PasswordHash
FROM Employees WHERE EmployeeCode = 'EMP001';

IF @AdminEmpId IS NOT NULL
BEGIN
    PRINT '   ✅ Admin employee found (EmployeeId: ' + CAST(@AdminEmpId AS VARCHAR) + ')';
    PRINT '   - PositionId: ' + ISNULL(CAST(@AdminEmpPosId AS VARCHAR), 'NULL');
    PRINT '   - Username: ' + ISNULL(@AdminUsername, 'NULL');
    PRINT '   - PasswordHash: ' + ISNULL(@AdminPassword, 'NULL');
    
    IF @AdminEmpPosId IS NULL
        PRINT '   ⚠️  WARNING: PositionId is NULL!';
    IF @AdminUsername IS NULL
        PRINT '   ⚠️  WARNING: Username is NULL!';
    IF @AdminPassword IS NULL
        PRINT '   ⚠️  WARNING: PasswordHash is NULL!';
END
ELSE
    PRINT '   ❌ Admin employee NOT FOUND!';
PRINT '';

-- 4. Check Permissions count
PRINT '4. Checking Permissions:';
DECLARE @PermCount INT;
SELECT @PermCount = COUNT(*) FROM Permissions;
PRINT '   ✅ ' + CAST(@PermCount AS VARCHAR) + ' permissions found';
PRINT '';

-- 5. Check PositionPermissions for Admin
PRINT '5. Checking PositionPermissions for Admin:';
DECLARE @PosPermCount INT;
IF @AdminPosId IS NOT NULL
BEGIN
    SELECT @PosPermCount = COUNT(*) FROM PositionPermissions WHERE PositionId = @AdminPosId;
    PRINT '   ✅ ' + CAST(@PosPermCount AS VARCHAR) + ' permissions assigned to Admin position';
    
    IF @PosPermCount = 0
        PRINT '   ⚠️  WARNING: No permissions assigned to Admin!';
END
ELSE
    PRINT '   ❌ Cannot check - Admin position not found';
PRINT '';

PRINT '============================================';
PRINT '  Summary';
PRINT '============================================';
IF @AdminEmpId IS NOT NULL AND @AdminUsername IS NOT NULL AND @AdminPassword IS NOT NULL AND @AdminEmpPosId IS NOT NULL AND @PosPermCount > 0
BEGIN
    PRINT '✅ All checks passed!';
    PRINT '';
    PRINT 'Login with:';
    PRINT '  Username: ' + @AdminUsername;
    PRINT '  Password: ' + @AdminPassword;
END
ELSE
BEGIN
    PRINT '❌ Some issues found! Run CompleteSetup.sql to fix!';
END
PRINT '============================================';
PRINT '';