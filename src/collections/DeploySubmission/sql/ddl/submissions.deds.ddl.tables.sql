IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Submissions')
	BEGIN
		EXEC('CREATE SCHEMA Submissions')
	END
GO

--------------------------------------------------------------------------------------
-- SubmissionEvents
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'SubmissionEvents' AND [schema_id] = SCHEMA_ID('Submissions'))
	BEGIN
		DROP TABLE [Submissions].[SubmissionEvents]
	END
GO

CREATE TABLE [Submissions].[SubmissionEvents]
(
	Id						bigint			PRIMARY KEY IDENTITY(1,1),
	IlrFileName				nvarchar(50)	NOT NULL,
	FileDateTime			datetime		NOT NULL,
	SubmittedDateTime		datetime		NOT NULL,
	ComponentVersionNumber	int				NOT NULL,
	UKPRN					bigint			NOT NULL,
	ULN						bigint			NOT NULL,
	LearnRefNumber			varchar(100)	NOT NULL,
    AimSeqNumber			bigint			NOT NULL,
	PriceEpisodeIdentifier	varchar(25)		NOT NULL,
	StandardCode			bigint			NULL,
	ProgrammeType			int				NULL,
	FrameworkCode			int				NULL,
	PathwayCode				int				NULL,
	ActualStartDate			date			NULL,
	PlannedEndDate			date			NULL,
	ActualEndDate			date			NULL,
	OnProgrammeTotalPrice	decimal(10,5)	NULL,
	CompletionTotalPrice	decimal(10,5)	NULL,
	NINumber				varchar(9)		NULL,
	CommitmentId			bigint			NULL,
	AcademicYear			varchar(4)    	NOT NULL,
	EmployerReferenceNumber int             NULL


)
GO

--------------------------------------------------------------------------------------
-- LastSeenVersion
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'LastSeenVersion' AND [schema_id] = SCHEMA_ID('Submissions'))
	BEGIN
		DROP TABLE [Submissions].[LastSeenVersion]
	END
GO

CREATE TABLE [Submissions].[LastSeenVersion]
(
	IlrFileName				nvarchar(50)	NOT NULL,
	FileDateTime			datetime		NOT NULL,	
	SubmittedDateTime		datetime		NOT NULL,
	ComponentVersionNumber	int				NOT NULL,
	UKPRN					bigint			NOT NULL,
	ULN						bigint			NOT NULL,
	LearnRefNumber			varchar(100)	NOT NULL,
    AimSeqNumber			bigint			NOT NULL,
	PriceEpisodeIdentifier	varchar(25)		NOT NULL,
	StandardCode			bigint			NULL,
	ProgrammeType			int				NULL,
	FrameworkCode			int				NULL,
	PathwayCode				int				NULL,
	ActualStartDate			date			NULL,
	PlannedEndDate			date			NULL,
	ActualEndDate			date			NULL,
	OnProgrammeTotalPrice	decimal(10,5)	NULL,
	CompletionTotalPrice	decimal(10,5)	NULL,
	NINumber				varchar(9)		NULL,
	CommitmentId			bigint			NULL,
	AcademicYear			varchar(4)    	NOT NULL,
	EmployerReferenceNumber int             NULL
)
GO