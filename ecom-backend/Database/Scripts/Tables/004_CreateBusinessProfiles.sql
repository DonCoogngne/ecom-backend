-- Database: EcomDb
-- Creates the BusinessProfiles table (one per user)

IF OBJECT_ID(N'dbo.BusinessProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessProfiles
    (
        BusinessProfileId INT             NOT NULL IDENTITY(1, 1),
        UserId            INT             NOT NULL,
        BusinessName      NVARCHAR(150)   NULL,
        Category          NVARCHAR(100)   NULL,
        Description       NVARCHAR(2000)  NULL,
        Website           NVARCHAR(300)   NULL,
        LogoUrl           NVARCHAR(500)   NULL,
        PrimaryColor      NVARCHAR(20)    NULL,
        SecondaryColor    NVARCHAR(20)    NULL,
        Location          NVARCHAR(200)   NULL,
        CreatedDate       DATETIME2       NOT NULL CONSTRAINT DF_BusinessProfiles_CreatedDate DEFAULT (SYSUTCDATETIME()),
        UpdatedDate       DATETIME2       NOT NULL CONSTRAINT DF_BusinessProfiles_UpdatedDate DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_BusinessProfiles PRIMARY KEY CLUSTERED (BusinessProfileId),
        CONSTRAINT UQ_BusinessProfiles_UserId UNIQUE (UserId),
        CONSTRAINT FK_BusinessProfiles_Users_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE CASCADE
    );
END
GO
