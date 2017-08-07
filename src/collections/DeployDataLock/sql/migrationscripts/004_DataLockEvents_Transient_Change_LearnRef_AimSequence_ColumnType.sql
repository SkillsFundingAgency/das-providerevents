

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

CREATE CLUSTERED INDEX [IDX_ValidationError_1] ON DataLock.ValidationError (
[Ukprn], [PriceEpisodeIdentifier])
with (drop_existing = on)
GO

ALTER TABLE  [DataLock].[ValidationError]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[ValidationError]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

CREATE CLUSTERED INDEX [IDX_ValidationError_1] ON DataLock.ValidationError (
[Ukprn], [LearnRefNumber], [AimSeqNumber], [PriceEpisodeIdentifier])
with (drop_existing = on)
GO


CREATE CLUSTERED INDEX [IDX_PriceEpisodeMatch_1] ON DataLock.PriceEpisodeMatch ([Ukprn], [PriceEpisodeIdentifier])
with (drop_existing = on)
GO

ALTER TABLE  [DataLock].[PriceEpisodeMatch]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[PriceEpisodeMatch]
ALTER COLUMN AimSeqNumber int NOT NULL
GO


CREATE CLUSTERED INDEX [IDX_PriceEpisodeMatch_1] ON DataLock.PriceEpisodeMatch ([Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber])
with (drop_existing = on)
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodePeriodMatch_1] ON DataLock.PriceEpisodePeriodMatch ([Ukprn], [PriceEpisodeIdentifier], [Period])
with (drop_existing = on)
GO

ALTER TABLE  [DataLock].[PriceEpisodePeriodMatch]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [DataLock].[PriceEpisodePeriodMatch]
ALTER COLUMN AimSeqNumber int NOT NULL
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodePeriodMatch_1] ON DataLock.PriceEpisodePeriodMatch ([Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber], [Period])
with (drop_existing = on)
GO


ALTER TABLE  [Reference].[DataLockEvents]
ALTER COLUMN LearnRefNumber	varchar(12) NOT NULL
GO

ALTER TABLE  [Reference].[DataLockEvents]
ALTER COLUMN AimSeqNumber int NOT NULL
GO


