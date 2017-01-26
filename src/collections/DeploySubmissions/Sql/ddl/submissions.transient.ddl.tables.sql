--====================================================================================
-- Component tables
--====================================================================================
IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Submissions')
	BEGIN
		EXEC('CREATE SCHEMA Submissions')
	END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- TaskLog
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='TaskLog' AND [schema_id] = SCHEMA_ID('Submissions'))
BEGIN
    DROP TABLE Submissions.TaskLog
END
GO

CREATE TABLE Submissions.TaskLog
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
-- SubmissionEvents
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'SubmissionEvents' AND [schema_id] = SCHEMA_ID('Submissions'))
	BEGIN
		DROP TABLE [Submissions].[SubmissionEvents]
	END
GO

CREATE TABLE [Submissions].[SubmissionEvents]
(
	Id						bigint			PRIMARY KEY,
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
	NINumber				varchar(9)		NULL
)
GO

--------------------------------------------------------------------------------------
-- LatestVersion
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'LatestVersion' AND [schema_id] = SCHEMA_ID('Submissions'))
	BEGIN
		DROP TABLE [Submissions].[LatestVersion]
	END
GO

CREATE TABLE [Submissions].[LatestVersion]
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
	NINumber				varchar(9)		NULL
)
GO

--====================================================================================
-- Reference tables
--====================================================================================
IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Reference')
	BEGIN
		EXEC('CREATE SCHEMA Reference')
	END
GO

--------------------------------------------------------------------------------------
-- Providers
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'Providers' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[Providers]
	END
GO

CREATE TABLE [Reference].[Providers]
(
	UKPRN	bigint	PRIMARY KEY
)
GO

--------------------------------------------------------------------------------------
-- LearningDeliveries
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'LearningDeliveries' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[LearningDeliveries]
	END
GO

CREATE TABLE [Reference].[LearningDeliveries]
(
	UKPRN				bigint			NOT NULL,
	LearnRefNumber		varchar(100)	NOT NULL,
    AimSeqNumber		bigint			NOT NULL,
	ULN					bigint			NOT NULL,
	NINumber			varchar(9)		NULL,
	ProgType			int				NULL,
	FworkCode			int				NULL,
	PwayCode			int				NULL,
	StdCode				int				NULL,
	LearnStartDate		date			NOT NULL,
	LearnPlanEndDate	date			NOT NULL,
	LearnActEndDate		date			NULL
)
GO

--------------------------------------------------------------------------------------
-- PriceEdpisodes
--------------------------------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'PriceEdpisodes' AND [schema_id] = SCHEMA_ID('Reference'))
	BEGIN
		DROP TABLE [Reference].[PriceEdpisodes]
	END
GO

CREATE TABLE [Reference].[PriceEdpisodes]
(
	Ukprn							bigint			NOT NULL,
	LearnRefNumber					varchar(100)	NOT NULL,
	PriceEpisodeAimSeqNumber		int				NOT NULL,
	EpisodeEffectiveTNPStartDate	date			NOT NULL,
	TNP1							decimal(10,5)	NULL,
	TNP2							decimal(10,5)	NULL,
	TNP3							decimal(10,5)	NULL,
	TNP4							decimal(10,5)	NULL
)