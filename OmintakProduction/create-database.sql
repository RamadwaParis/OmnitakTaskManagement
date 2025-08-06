-- Script to create OmniTaskDB database if it doesn't exist
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OmniTaskDB')
BEGIN
    CREATE DATABASE OmniTaskDB;
    PRINT 'Database OmniTaskDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database OmniTaskDB already exists.';
END
GO
