IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='DataLock')
BEGIN
    EXEC('CREATE SCHEMA DataLock')
END
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
    [PriceEpisodeIdentifier] varchar(25) NOT NULL,
    [CollectionPeriodName] varchar(8) NOT NULL,
    [CollectionPeriodMonth] int NOT NULL,
    [CollectionPeriodYear] int NOT NULL
)
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
    [CommitmentId] varchar(50) NOT NULL,
    [CollectionPeriodName] varchar(8) NOT NULL,
    [CollectionPeriodMonth] int NOT NULL,
    [CollectionPeriodYear] int NOT NULL,
	[IsSuccess] bit NOT NULL
)
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
    [VersionId] bigint NOT NULL,
    [Period] int NOT NULL,
    [Payable] bit NOT NULL,
	[TransactionType] int NOT NULL,
    [CollectionPeriodName] varchar(8) NOT NULL,
    [CollectionPeriodMonth] int NOT NULL,
    [CollectionPeriodYear] int NOT NULL
)
GO
