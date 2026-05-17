IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND name = N'IsDeleted')
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [IsDeleted] BIT NOT NULL DEFAULT 0;
END
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND name = N'IsActive')
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [IsActive] BIT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = N'IsDeleted')
BEGIN
    ALTER TABLE [dbo].[Departments] ADD [IsDeleted] BIT NOT NULL DEFAULT 0;
END
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = N'IsActive')
BEGIN
    ALTER TABLE [dbo].[Departments] ADD [IsActive] BIT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Positions]') AND name = N'IsDeleted')
BEGIN
    ALTER TABLE [dbo].[Positions] ADD [IsDeleted] BIT NOT NULL DEFAULT 0;
END
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Positions]') AND name = N'IsActive')
BEGIN
    ALTER TABLE [dbo].[Positions] ADD [IsActive] BIT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Teams]') AND name = N'IsDeleted')
BEGIN
    ALTER TABLE [dbo].[Teams] ADD [IsDeleted] BIT NOT NULL DEFAULT 0;
END
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Teams]') AND name = N'IsActive')
BEGIN
    ALTER TABLE [dbo].[Teams] ADD [IsActive] BIT NULL DEFAULT 1;
END
GO
