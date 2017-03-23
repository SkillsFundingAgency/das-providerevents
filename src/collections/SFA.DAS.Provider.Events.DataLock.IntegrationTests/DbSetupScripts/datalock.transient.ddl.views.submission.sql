IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='DataLock')
BEGIN
    EXEC('CREATE SCHEMA DataLock')
END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- vw_Providers
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_Providers' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP VIEW DataLock.vw_Providers
END
GO

CREATE VIEW DataLock.vw_Providers
AS
SELECT
    p.UKPRN
FROM Valid.LearningProvider p
GO
