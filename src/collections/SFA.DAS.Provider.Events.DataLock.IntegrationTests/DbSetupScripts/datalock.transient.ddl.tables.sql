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
    [Ukprn] bigint,
    [LearnRefNumber] varchar(100),
    [AimSeqNumber] bigint,
    [RuleId] varchar(50),
    [PriceEpisodeIdentifier] varchar(25) NOT NULL
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
    [LearnRefNumber] varchar(100) NOT NULL,
    [AimSeqNumber] bigint NOT NULL,
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
    [LearnRefNumber] varchar(100) NOT NULL,
    [AimSeqNumber] bigint NOT NULL,
    [CommitmentId] bigint NOT NULL,
    [VersionId] varchar(25) NOT NULL,
    [Period] int NOT NULL,
    [Payable] bit NOT NULL,
	[TransactionType] int NOT NULL
)
GO

CREATE CLUSTERED INDEX [IDX_PriceEpisodePeriodMatch_1] ON DataLock.PriceEpisodePeriodMatch ([Ukprn], [PriceEpisodeIdentifier], [LearnRefNumber], [AimSeqNumber], [Period])
GO
