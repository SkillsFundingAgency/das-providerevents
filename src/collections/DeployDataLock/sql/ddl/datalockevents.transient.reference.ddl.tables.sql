IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Reference')
	BEGIN
		EXEC('CREATE SCHEMA Reference')
	END
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
	Id							bigint				NOT NULL,
	ProcessDateTime				datetime			NOT NULL,
	Status						int					NOT NULL,
	IlrFileName					nvarchar(50)		NOT NULL,
	SubmittedDateTime		    datetime			NOT NULL,
	AcademicYear				varchar(4)    		NOT NULL,
	UKPRN						bigint				NOT NULL,
	ULN							bigint				NOT NULL,
	LearnRefNumber				varchar(100)		NOT NULL,
    AimSeqNumber				bigint				NOT NULL,
	PriceEpisodeIdentifier		varchar(25)			NOT NULL,
	CommitmentId				bigint				NOT NULL,
	EmployerAccountId			bigint				NOT NULL,
	EventSource					int					NOT NULL,
	HasErrors					bit					NOT NULL,
	IlrStartDate				date				NULL,
	IlrStandardCode				bigint				NULL,
	IlrProgrammeType			int					NULL,
	IlrFrameworkCode			int					NULL,
	IlrPathwayCode				int					NULL,
	IlrTrainingPrice			decimal(12,5)		NULL,
	IlrEndpointAssessorPrice	decimal(12,5)		NULL,
	IlrPriceEffectiveFromDate	date				NULL,
	IlrPriceEffectiveToDate		date				NULL,
	DataLockEventId				uniqueidentifier	NOT NULL
	CONSTRAINT [PK_Reference_DataLockEvents] PRIMARY KEY NONCLUSTERED (Id),
	INDEX [IX_Reference_DataLockEvents_UKPRN]  CLUSTERED (UKPRN)
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
	DataLockEventId			uniqueidentifier			NOT NULL,
	CollectionPeriodName	varchar(8)		NOT NULL,
	CollectionPeriodMonth	int				NOT NULL,
	CollectionPeriodYear	int				NOT NULL,
	CommitmentVersion		bigint			NOT NULL,
	IsPayable				bit				NOT NULL,
	TransactionType			int				NOT NULL,
	INDEX [IX_Reference_DataLockEventPeriods_DataLockEventId] CLUSTERED (DataLockEventId)
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
	DataLockEventId				uniqueidentifier			NOT NULL,
	CommitmentVersion			bigint			NOT NULL,
	CommitmentStartDate			date			NOT NULL,
	CommitmentStandardCode		bigint			NULL,
	CommitmentProgrammeType		int				NULL,
	CommitmentFrameworkCode		int				NULL,
	CommitmentPathwayCode		int				NULL,
	CommitmentNegotiatedPrice	decimal(12,5)	NOT NULL,
	CommitmentEffectiveDate		date			NOT NULL,
	INDEX [IX_Reference_DataLockEventCommitmentVersions_DataLockEventId] CLUSTERED (DataLockEventId)
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
	DataLockEventId			uniqueidentifier			NOT NULL,
	ErrorCode				varchar(15)		NOT NULL,
	SystemDescription		nvarchar(255)	NOT NULL,
	PRIMARY KEY NONCLUSTERED (DataLockEventId, ErrorCode),
	INDEX [IX_Reference_DataLockEventErrors_DataLockEventId] CLUSTERED (DataLockEventId)
)
GO

