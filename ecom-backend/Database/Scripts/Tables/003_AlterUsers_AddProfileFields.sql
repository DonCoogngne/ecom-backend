-- Database: EcomDb
-- Adds profile + security fields to the Users table

IF COL_LENGTH(N'dbo.Users', N'Phone') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD Phone NVARCHAR(30) NULL;
END
GO

IF COL_LENGTH(N'dbo.Users', N'TwoFactorEnabled') IS NULL
BEGIN
    ALTER TABLE dbo.Users
        ADD TwoFactorEnabled BIT NOT NULL
            CONSTRAINT DF_Users_TwoFactorEnabled DEFAULT (0);
END
GO
