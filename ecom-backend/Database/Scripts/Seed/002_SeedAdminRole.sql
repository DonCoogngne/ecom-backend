-- Database: EcomDb
-- Seed Admin role

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Admin')
BEGIN
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Admin');
END
GO
