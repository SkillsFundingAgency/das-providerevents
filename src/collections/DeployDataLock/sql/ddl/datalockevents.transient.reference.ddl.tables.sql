IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Reference')
	BEGIN
		EXEC('CREATE SCHEMA Reference')
	END
GO

--------------------------------------------------------------------------------------
-- IdentifierSeed
--------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='IdentifierSeed' AND [schema_id] = SCHEMA_ID('Reference'))
BEGIN
    DROP TABLE Reference.IdentifierSeed
END
GO

CREATE TABLE Reference.IdentifierSeed
(
    IdentifierName		varchar(50)		PRIMARY KEY,
	MaxIdInDeds			bigint			NOT NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEvents
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEvents' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[DataLockEvents]
	END
GO

CREATE TABLE [Reference].[DataLockEvents]
(
	Id							bigint			PRIMARY KEY,
	ProcessDateTime				datetime		NOT NULL,
	IlrFileName					nvarchar(50)	NOT NULL,
	SubmittedDateTime		    datetime		NOT NULL,
	AcademicYear				varchar(4)    	NOT NULL,
	UKPRN						bigint			NOT NULL,
	ULN							bigint			NOT NULL,
	LearnRefNumber				varchar(100)	NOT NULL,
    AimSeqNumber				bigint			NOT NULL,
	PriceEpisodeIdentifier		varchar(25)		NOT NULL,
	CommitmentId				bigint			NOT NULL,
	EmployerAccountId			bigint			NOT NULL,
	EventSource					int				NOT NULL,
	HasErrors					bit				NOT NULL,
	IlrStartDate				date			NULL,
	IlrStandardCode				bigint			NULL,
	IlrProgrammeType			int				NULL,
	IlrFrameworkCode			int				NULL,
	IlrPathwayCode				int				NULL,
	IlrTrainingPrice			decimal(12,5)	NULL,
	IlrEndpointAssessorPrice	decimal(12,5)	NULL,
	IlrPriceEffectiveDate		date			NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEventPeriods
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventPeriods' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[DataLockEventPeriods]
	END
GO

CREATE TABLE [Reference].[DataLockEventPeriods]
(
	DataLockEventId			bigint			NOT NULL,
	CollectionPeriodName	varchar(8)		NOT NULL,
	CollectionPeriodMonth	int				NOT NULL,
	CollectionPeriodYear	int				NOT NULL,
	CommitmentVersion		bigint			NOT NULL,
	IsPayable				bit				NOT NULL,
	TransactionType			int				NOT NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEventCommitmentVersions
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventCommitmentVersions' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[DataLockEventCommitmentVersions]
	END
GO

CREATE TABLE [Reference].[DataLockEventCommitmentVersions]
(
	DataLockEventId				bigint			NOT NULL,
	CommitmentVersion			bigint			NOT NULL,
	CommitmentStartDate			date			NOT NULL,
	CommitmentStandardCode		bigint			NULL,
	CommitmentProgrammeType		int				NULL,
	CommitmentFrameworkCode		int				NULL,
	CommitmentPathwayCode		int				NULL,
	CommitmentNegotiatedPrice	decimal(12,5)	NOT NULL,
	CommitmentEffectiveDate		date			NOT NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEventErrors
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventErrors' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[DataLockEventErrors]
	END
GO

CREATE TABLE [Reference].[DataLockEventErrors]
(
	DataLockEventId			bigint			NOT NULL,
	ErrorCode				varchar(15)		NOT NULL,
	SystemDescription		nvarchar(255)	NOT NULL,
	PRIMARY KEY (DataLockEventId, ErrorCode)
)
GO

