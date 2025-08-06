-- Comprehensive database cleanup script
USE OmniTaskDB;
GO

-- Disable foreign key constraints
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL"
GO

-- Drop all foreign key constraints
DECLARE @sql NVARCHAR(MAX) = ''
SELECT @sql = @sql + 'ALTER TABLE [' + SCHEMA_NAME(schema_id) + '].[' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
FROM sys.foreign_keys

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql
    PRINT 'Foreign key constraints dropped.'
END

-- Drop all tables except migrations history
SET @sql = ''
SELECT @sql = @sql + 'DROP TABLE [' + SCHEMA_NAME(schema_id) + '].[' + name + '];' + CHAR(13)
FROM sys.tables 
WHERE name != '__EFMigrationsHistory'

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql
    PRINT 'All tables dropped.'
END

-- Clear migrations history
DELETE FROM __EFMigrationsHistory;
PRINT 'Migrations history cleared.'

-- Check if any objects remain
SELECT 'Remaining objects:' as Message
SELECT name, type_desc FROM sys.objects WHERE type IN ('U', 'V', 'P', 'FN') AND name != '__EFMigrationsHistory'
GO
