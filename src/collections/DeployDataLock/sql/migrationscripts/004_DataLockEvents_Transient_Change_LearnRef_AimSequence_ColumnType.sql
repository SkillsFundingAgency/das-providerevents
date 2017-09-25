

--[DataLockEvents]
ALTER TABLE  [DataLockEvents].[DataLockEvents]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLockEvents].[DataLockEvents]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

ALTER TABLE  [DataLockEvents].[DataLockEventsData]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLockEvents].[DataLockEventsData]
ALTER COLUMN AimSeqNumber int NOT NULL
GO



-- Dropping the index to modify the dependent columns - will be added later
IF EXISTS (SELECT * FROM sys.indexes WHERE name='IDX_ValidationError_1' 
	AND object_id = OBJECT_ID('DataLock.ValidationError', 'Table'))
BEGIN
	DROP INDEX [IDX_ValidationError_1] ON [DataLock].[ValidationError];
END

ALTER TABLE  [DataLock].[ValidationError]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[ValidationError]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

CREATE CLUSTERED INDEX [IDX_ValidationError_1] ON DataLock.ValidationError (
	[Ukprn], [LearnRefNumber], [AimSeqNumber], [PriceEpisodeIdentifier])
GO



-- Dropping the index to modify the dependent columns - will be added later
IF EXISTS (SELECT * FROM sys.indexes WHERE name='IDX_PriceEpisodeMatch_1' 
	AND object_id = OBJECT_ID('DataLock.PriceEpisodeMatch', 'Table'))
BEGIN
	DROP INDEX [IDX_PriceEpisodeMatch_1] ON [DataLock].[PriceEpisodeMatch];
END

ALTER TABLE  [DataLock].[PriceEpisodeMatch]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[PriceEpisodeMatch]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodeMatch_1] ON DataLock.PriceEpisodeMatch (
	[Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber])
GO



-- Dropping the index to modify the dependent columns - will be added later
IF EXISTS (SELECT * FROM sys.indexes WHERE name='IDX_PriceEpisodePeriodMatch_1' 
	AND object_id = OBJECT_ID('DataLock.PriceEpisodePeriodMatch', 'Table'))
BEGIN
	DROP INDEX [IDX_PriceEpisodePeriodMatch_1] ON [DataLock].[PriceEpisodePeriodMatch];
END

ALTER TABLE  [DataLock].[PriceEpisodePeriodMatch]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[PriceEpisodePeriodMatch]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodePeriodMatch_1] ON DataLock.PriceEpisodePeriodMatch (
	[Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber], [Period])
GO


-- Non-indexed columns
ALTER TABLE  [Reference].[DataLockEvents]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [Reference].[DataLockEvents]
ALTER COLUMN AimSeqNumber int NOT NULL
GO


