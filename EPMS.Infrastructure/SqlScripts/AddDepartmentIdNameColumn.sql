IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'DepartmentIdName')
BEGIN
    ALTER TABLE [dbo].[Departments] ADD [DepartmentIdName] NVARCHAR(50) NULL;
END
GO
