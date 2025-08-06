-- Script to completely drop and recreate OmniTaskDB database
USE master;
GO

-- Close all connections to the database
ALTER DATABASE OmniTaskDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Drop the database
DROP DATABASE OmniTaskDB;
GO

-- Recreate the database
CREATE DATABASE OmniTaskDB;
GO

PRINT 'Database OmniTaskDB has been completely recreated.';
GO
