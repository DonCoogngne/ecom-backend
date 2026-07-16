-- Database: EcomDb
-- Creates the Invoices table (billing history for a subscription)

IF OBJECT_ID(N'dbo.Invoices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices
    (
        InvoiceId      INT             NOT NULL IDENTITY(1, 1),
        SubscriptionId INT             NOT NULL,
        InvoiceNumber  NVARCHAR(50)    NOT NULL,
        InvoiceDate    DATETIME2       NOT NULL CONSTRAINT DF_Invoices_InvoiceDate DEFAULT (SYSUTCDATETIME()),
        Amount         DECIMAL(10, 2)  NOT NULL CONSTRAINT DF_Invoices_Amount DEFAULT (0),
        Status         NVARCHAR(30)    NOT NULL CONSTRAINT DF_Invoices_Status DEFAULT (N'Paid'),

        CONSTRAINT PK_Invoices PRIMARY KEY CLUSTERED (InvoiceId),
        CONSTRAINT FK_Invoices_Subscriptions_SubscriptionId
            FOREIGN KEY (SubscriptionId) REFERENCES dbo.Subscriptions (SubscriptionId)
            ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX IX_Invoices_SubscriptionId
        ON dbo.Invoices (SubscriptionId);
END
GO
