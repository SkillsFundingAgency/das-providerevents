IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'DataLockEvents')
	BEGIN
		EXEC('CREATE SCHEMA DataLockEvents')
	END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- TaskLog
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='TaskLog' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
BEGIN
    DROP TABLE DataLockEvents.TaskLog
END
GO

CREATE TABLE DataLockEvents.TaskLog
(
    [TaskLogId] uniqueidentifier NOT NULL DEFAULT(NEWID()),
    [DateTime] datetime NOT NULL DEFAULT(GETDATE()),
    [Level] nvarchar(10) NOT NULL,
    [Logger] nvarchar(512) NOT NULL,
    [Message] nvarchar(1024) NOT NULL,
    [Exception] nvarchar(max) NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEvents
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEvents' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP TABLE [DataLockEvents].[DataLockEvents]
	END
GO

CREATE TABLE [DataLockEvents].[DataLockEvents]
(
	Id							bigint				PRIMARY KEY IDENTITY(1,1),
	DataLockEventId				uniqueidentifier	NOT NULL,
	ProcessDateTime				datetime			NOT NULL,
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
	IlrPriceEffectiveFromDate	date				NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEventPeriods
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventPeriods' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP TABLE [DataLockEvents].[DataLockEventPeriods]
	END
GO

CREATE TABLE [DataLockEvents].[DataLockEventPeriods]
(
	DataLockEventId			uniqueidentifier			NOT NULL,
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
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventCommitmentVersions' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP TABLE [DataLockEvents].[DataLockEventCommitmentVersions]
	END
GO

CREATE TABLE [DataLockEvents].[DataLockEventCommitmentVersions]
(
	DataLockEventId				uniqueidentifier			NOT NULL,
	CommitmentVersion			bigint			NOT NULL,
	CommitmentStartDate			date			NULL,
	CommitmentStandardCode		bigint			NULL,
	CommitmentProgrammeType		int				NULL,
	CommitmentFrameworkCode		int				NULL,
	CommitmentPathwayCode		int				NULL,
	CommitmentNegotiatedPrice	decimal(12,5)	NULL,
	CommitmentEffectiveDate		date			NULL
)
GO

--------------------------------------------------------------------------------------
-- DataLockEventErrors
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventErrors' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP TABLE [DataLockEvents].[DataLockEventErrors]
	END
GO

CREATE TABLE [DataLockEvents].[DataLockEventErrors]
(
	DataLockEventId			uniqueidentifier			NOT NULL,
	ErrorCode				varchar(15)		NOT NULL,
	SystemDescription		nvarchar(255)	NOT NULL,
	PRIMARY KEY (DataLockEventId, ErrorCode)
)
GO



--------------------------------------------------------------------------------------
-- DataLockEventErrors
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventsData' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP TABLE [DataLockEvents].[DataLockEventsData]
	END
GO

CREATE TABLE [DataLockEvents].[DataLockEventsData]
(
	
	Ukprn bigint  NOT NULL,
	PriceEpisodeIdentifier varchar(25)  NULL,
	LearnRefNumber varchar(100)  NULL,
	AimSeqNumber bigint  NULL,
	CommitmentId bigint  NULL,
	IsSuccess bit  NULL,
	IlrFilename		nvarchar(50)	NULL,
	SubmittedTime	datetime NULL,	
	Uln	bigint	NULL,
	IlrStartDate				date			NULL,
	IlrStandardCode				bigint			NULL,
	IlrProgrammeType			int				NULL,
	IlrFrameworkCode			int				NULL,
	IlrPathwayCode				int				NULL,
	IlrTrainingPrice			decimal(12,5)	NULL,
	IlrEndpointAssessorPrice	decimal(12,5)	NULL,
	IlrPriceEffectiveFromDate		date			NULL,
	CommitmentVersionId bigint NULL,
	Period int NULL,
	Payable bit NULL,
	TransactionType int NULL,
	EmployerAccountId bigint  NULL,
	CommitmentStartDate date  NULL,
	CommitmentStandardCode bigint NULL,
	CommitmentProgrammeType int NULL,
	CommitmentFrameworkCode int NULL,
	CommitmentPathwayCode int NULL,
	CommitmentNegotiatedPrice decimal(15, 2) NULL,
	CommitmentEffectiveDate date  NULL,
	RuleId varchar(50) NULL

)
GO


IF EXISTS(SELECT [object_id] FROM sys.indexes WHERE [name]='IX_DataLockEventsData_UKPRN')
BEGIN
		DROP INDEX [IX_DataLockEventsData_UKPRN] ON [DataLockEvents].[DataLockEventsData]
END
GO
 
CREATE INDEX [IX_DataLockEventsData_UKPRN] ON [DataLockEvents].[DataLockEventsData] (UKPRN)
GO