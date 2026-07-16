-- Database: EcomDb
-- Login provider registry (Email, Google, Apple) per user.
-- One row per (UserId, Provider). External providers store ProviderUserId
-- (e.g. Google 'sub'); the Email provider stores the BCrypt PasswordHash.

IF OBJECT_ID(N'dbo.UserAuthentication', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserAuthentication
    (
        UserAuthenticationId INT            NOT NULL IDENTITY(1, 1),
        UserId               INT            NOT NULL,
        Provider             NVARCHAR(30)   NOT NULL,
        ProviderUserId       NVARCHAR(256)  NULL,
        PasswordHash         NVARCHAR(500)  NULL,
        CreatedDate          DATETIME2      NOT NULL CONSTRAINT DF_UserAuthentication_CreatedDate DEFAULT (SYSUTCDATETIME()),
        LastLoginDate        DATETIME2      NULL,

        CONSTRAINT PK_UserAuthentication PRIMARY KEY CLUSTERED (UserAuthenticationId),
        CONSTRAINT UQ_UserAuthentication_User_Provider UNIQUE (UserId, Provider),
        CONSTRAINT CK_UserAuthentication_Provider
            CHECK (Provider IN (N'Email', N'Google', N'Apple')),
        CONSTRAINT FK_UserAuthentication_Users_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE CASCADE
    );

    -- A given external identity (provider + subject) can map to only one user.
    CREATE UNIQUE NONCLUSTERED INDEX UX_UserAuthentication_Provider_ProviderUserId
        ON dbo.UserAuthentication (Provider, ProviderUserId)
        WHERE ProviderUserId IS NOT NULL;
END
GO
