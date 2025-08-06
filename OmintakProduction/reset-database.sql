-- Script to reset the database and handle existing tables
USE OmniTaskDB;

-- Drop all foreign key constraints first
DECLARE @sql NVARCHAR(MAX) = ''
SELECT @sql = @sql + 'ALTER TABLE [' + SCHEMA_NAME(schema_id) + '].[' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + ']' + CHAR(13)
FROM sys.foreign_keys

EXEC sp_executesql @sql

-- Drop all tables
SET @sql = ''
SELECT @sql = @sql + 'DROP TABLE [' + SCHEMA_NAME(schema_id) + '].[' + name + ']' + CHAR(13)
FROM sys.tables

EXEC sp_executesql @sql

-- Drop the migrations history table if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__EFMigrationsHistory]
