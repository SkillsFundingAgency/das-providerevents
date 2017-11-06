IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='DataLock')
BEGIN
    EXEC('CREATE SCHEMA DataLock')
END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- TaskLog
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='TaskLog' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP TABLE DataLock.TaskLog
END
GO

CREATE TABLE DataLock.TaskLog
(
    [TaskLogId] uniqueidentifier NOT NULL DEFAULT(NEWID()),
    [DateTime] datetime NOT NULL DEFAULT(GETDATE()),
    [Level] nvarchar(10) NOT NULL,
    [Logger] nvarchar(512) NOT NULL,
    [Message] nvarchar(1024) NOT NULL,
    [Exception] nvarchar(max) NULL
)
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- ValidationError
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='ValidationError' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP TABLE DataLock.ValidationError
END
GO

CREATE TABLE DataLock.ValidationError
(
    [Ukprn] [bigint] NULL,
	[LearnRefNumber] [varchar](12) NULL,
	[AimSeqNumber] [int] NULL,
	[RuleId] [varchar](50) NULL,
	[PriceEpisodeIdentifier] [varchar](25) NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL
)
GO

CREATE CLUSTERED INDEX [IDX_ValidationError_1] ON DataLock.ValidationError ([Ukprn], [LearnRefNumber], [AimSeqNumber], [PriceEpisodeIdentifier])
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- PriceEpisodeMatch
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='PriceEpisodeMatch' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP TABLE DataLock.PriceEpisodeMatch
END
GO

CREATE TABLE DataLock.PriceEpisodeMatch
(
    [Ukprn] bigint NOT NULL,
    [PriceEpisodeIdentifier] varchar(25) NOT NULL,
    [LearnRefNumber] varchar(12) NOT NULL,
    [AimSeqNumber] int NOT NULL,
    [CommitmentId] bigint NOT NULL,
	[IsSuccess] bit NOT NULL
)
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodeMatch_1] ON DataLock.PriceEpisodeMatch ([Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber])
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- PriceEpisodePeriodMatch
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='PriceEpisodePeriodMatch' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP TABLE DataLock.PriceEpisodePeriodMatch
END
GO

CREATE TABLE DataLock.PriceEpisodePeriodMatch
(
    [Ukprn] bigint NOT NULL,
    [PriceEpisodeIdentifier] varchar(25) NOT NULL,
    [LearnRefNumber] varchar(12) NOT NULL,
    [AimSeqNumber] int NOT NULL,
    [CommitmentId] bigint NOT NULL,
    [VersionId] varchar(25) NOT NULL,
    [Period] int NOT NULL,
    [Payable] bit NOT NULL,
	[TransactionType] int NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL

)
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodePeriodMatch_1] ON DataLock.PriceEpisodePeriodMatch ([Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber], [Period])
GO

