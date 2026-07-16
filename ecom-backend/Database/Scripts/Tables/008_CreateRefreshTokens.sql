-- Database: EcomDb
-- Refresh tokens for rotation. Only the SHA-256 hash of each token is stored.
-- NOTE: UserId is INT to match the existing dbo.Users primary key
-- (the sample used UNIQUEIDENTIFIER; RefreshTokenId remains a GUID).

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        RefreshTokenId      UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_RefreshTokens_Id DEFAULT (NEWID()),
        UserId              INT              NOT NULL,
        TokenHash           NVARCHAR(500)    NOT NULL,
        ExpiresAt           DATETIME2        NOT NULL,
        CreatedAt           DATETIME2        NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT (SYSUTCDATETIME()),
        RevokedAt           DATETIME2        NULL,
        ReplacedByTokenHash NVARCHAR(500)    NULL,
        CreatedByIp         NVARCHAR(45)     NULL,
        RevokedByIp         NVARCHAR(45)     NULL,
        DeviceName          NVARCHAR(200)    NULL,
        IsActive            AS (
            CASE
                WHEN RevokedAt IS NULL
                 AND ExpiresAt > SYSUTCDATETIME()
                THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END
        ),

        CONSTRAINT PK_RefreshTokens PRIMARY KEY CLUSTERED (RefreshTokenId),
        CONSTRAINT FK_RefreshTokens_Users_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE CASCADE
    );

    CREATE UNIQUE NONCLUSTERED INDEX UX_RefreshTokens_TokenHash
        ON dbo.RefreshTokens (TokenHash);

    CREATE NONCLUSTERED INDEX IX_RefreshTokens_UserId
        ON dbo.RefreshTokens (UserId);
END
GO
