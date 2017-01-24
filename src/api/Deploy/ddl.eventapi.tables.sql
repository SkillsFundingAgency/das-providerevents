IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE name='EventApi')
	BEGIN
		EXEC('CREATE SCHEMA EventApi')
	END
GO

IF NOT EXISTS (SELECT [object_id] FROM sys.tables WHERE name='Logs' AND [schema_id] = SCHEMA_ID('EventApi'))
	BEGIN
		CREATE TABLE EventApi.Logs
		(
			LogDateTime datetime, 
			LogLevel varchar(10), 
			Message varchar(max), 
			Logger varchar(255), 
			CallSite varchar(255), 
			Exception varchar(max), 
			ActivityId varchar(50)
		)
	END
GO