-- Database: EcomDb
-- Creates the Subscriptions table (one per user)

IF OBJECT_ID(N'dbo.Subscriptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Subscriptions
    (
        SubscriptionId    INT           NOT NULL IDENTITY(1, 1),
        UserId            INT           NOT NULL,
        PlanName          NVARCHAR(50)  NOT NULL CONSTRAINT DF_Subscriptions_PlanName DEFAULT (N'Free'),
        FreeCredits       INT           NOT NULL CONSTRAINT DF_Subscriptions_FreeCredits DEFAULT (25),
        RemainingCredits  INT           NOT NULL CONSTRAINT DF_Subscriptions_RemainingCredits DEFAULT (25),
        CreatedDate       DATETIME2     NOT NULL CONSTRAINT DF_Subscriptions_CreatedDate DEFAULT (SYSUTCDATETIME()),
        UpdatedDate       DATETIME2     NOT NULL CONSTRAINT DF_Subscriptions_UpdatedDate DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Subscriptions PRIMARY KEY CLUSTERED (SubscriptionId),
        CONSTRAINT UQ_Subscriptions_UserId UNIQUE (UserId),
        CONSTRAINT FK_Subscriptions_Users_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE CASCADE
    );
END
GO
