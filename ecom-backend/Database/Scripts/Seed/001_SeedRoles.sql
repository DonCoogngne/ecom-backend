-- Database: EcomDb
-- Seed default Customer role (required for signup)

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Customer')
BEGIN
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Customer');
END
GO
