-- Database: EcomDb
-- Seed default roles required by auth (Customer = default signup role)

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Customer')
BEGIN
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Customer');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Admin')
BEGIN
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Admin');
END
GO
