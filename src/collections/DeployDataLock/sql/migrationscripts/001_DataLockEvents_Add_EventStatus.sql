IF NOT EXISTS(SELECT [column_id] FROM sys.columns WHERE [name] = 'Status' AND [object_id] = OBJECT_ID('DataLock.DataLockEvents'))
	BEGIN
		ALTER TABLE [DataLock].[DataLockEvents]
		ADD [Status] int NULL
	END
GO

DECLARE @RequireBackfill bit = (SELECT TOP 1 is_nullable FROM sys.columns WHERE [name] = 'Status' AND [object_id] = OBJECT_ID('DataLock.DataLockEvents'))
IF (@RequireBackfill = 1) 
	BEGIN
		CREATE TABLE #NewEvents
		(
			PriceEpisodeIdentifier varchar(25),
			LearnRefNumber varchar(100),
			UKPRN bigint,
			EventId bigint
		)

		INSERT INTO #NewEvents
		SELECT PriceEpisodeIdentifier, LearnRefNumber, UKPRN, MIN(Id)
		FROM DataLock.DataLockEvents
		GROUP BY PriceEpisodeIdentifier, LearnRefNumber, UKPRN

		UPDATE [DataLock].[DataLockEvents]
		SET [Status] = 2

		UPDATE [DataLock].[DataLockEvents]
		SET [Status] = 1
		WHERE Id IN (SELECT EventId FROM #NewEvents)

		ALTER TABLE [DataLock].[DataLockEvents]
		ALTER COLUMN [Status] int NOT NULL

		DROP TABLE #NewEvents
	END
GO