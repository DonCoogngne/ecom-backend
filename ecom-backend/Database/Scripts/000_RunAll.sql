-- Master script: create EcomDb schema objects in order
-- Run against LocalDB / SQL Server while connected to database EcomDb
-- Example:
--   sqlcmd -S "(localdb)\mssqllocaldb" -d EcomDb -i 000_RunAll.sql
-- Or create the database first if needed:
--   CREATE DATABASE EcomDb;

:r .\Tables\001_CreateRoles.sql
:r .\Tables\002_CreateUsers.sql
:r .\Tables\003_AlterUsers_AddProfileFields.sql
:r .\Tables\004_CreateBusinessProfiles.sql
:r .\Tables\005_CreateSubscriptions.sql
:r .\Tables\006_CreateInvoices.sql
:r .\Tables\007_CreateUserAuthentication.sql
:r .\Tables\008_CreateRefreshTokens.sql
:r .\Seed\001_SeedRoles.sql
:r .\Seed\002_SeedAdminRole.sql
