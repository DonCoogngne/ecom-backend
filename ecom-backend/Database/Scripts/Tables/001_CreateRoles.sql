-- Database: EcomDb
-- Creates the Roles table (run before Users)

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        RoleId   INT            NOT NULL IDENTITY(1, 1),
        RoleName NVARCHAR(50)   NOT NULL,

        CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId),
        CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
    );
END
GO
