-- Database: EcomDb
-- Creates the Users table (requires Roles)

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId          INT             NOT NULL IDENTITY(1, 1),
        FirstName       NVARCHAR(100)   NOT NULL,
        LastName        NVARCHAR(100)   NOT NULL,
        Email           NVARCHAR(256)   NOT NULL,
        PasswordHash    NVARCHAR(500)   NULL,
        ProfileImageUrl NVARCHAR(500)   NULL,
        CreatedDate     DATETIME2       NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT (SYSUTCDATETIME()),
        IsActive        BIT             NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        RoleId          INT             NOT NULL,
        GoogleId        NVARCHAR(128)   NULL,

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT FK_Users_Roles_RoleId
            FOREIGN KEY (RoleId) REFERENCES dbo.Roles (RoleId)
            ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX IX_Users_GoogleId
        ON dbo.Users (GoogleId);
END
GO
