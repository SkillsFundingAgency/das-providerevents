-------------------------------------------------------------
---		SCHEMAS
-------------------------------------------------------------
IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Submissions')
	BEGIN
		EXEC('CREATE SCHEMA Submissions')
	END
GO

-------------------------------------------------------------
---		Submissions.SubmissionEvents
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'SubmissionEvents' AND [schema_id] = SCHEMA_ID('Submissions'))
	BEGIN
		DROP TABLE [Submissions].[SubmissionEvents]
	END
GO
CREATE TABLE [Submissions].[SubmissionEvents](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IlrFileName] [nvarchar](50) NOT NULL,
	[FileDateTime] [datetime] NOT NULL,
	[SubmittedDateTime] [datetime] NOT NULL,
	[ComponentVersionNumber] [int] NOT NULL,
	[UKPRN] [bigint] NOT NULL,
	[ULN] [bigint] NOT NULL,
	[LearnRefNumber] [varchar](100) NOT NULL,
	[AimSeqNumber] [bigint] NOT NULL,
	[PriceEpisodeIdentifier] [varchar](25) NOT NULL,
	[StandardCode] [bigint] NULL,
	[ProgrammeType] [int] NULL,
	[FrameworkCode] [int] NULL,
	[PathwayCode] [int] NULL,
	[ActualStartDate] [date] NULL,
	[PlannedEndDate] [date] NULL,
	[ActualEndDate] [date] NULL,
	[OnProgrammeTotalPrice] [decimal](10, 5) NULL,
	[CompletionTotalPrice] [decimal](10, 5) NULL,
	[NINumber] [varchar](9) NULL,
	[CommitmentId] [bigint] NULL,
	[AcademicYear] [varchar](4) NOT NULL,
	[EmployerReferenceNumber] [int] NULL,
	[EPAOrgId] [VARCHAR](7) NULL,
	[GivenNames] [varchar](100) NOT NULL,
	[FamilyName] [varchar](100) NOT NULL,
	[CompStatus] [int] NULL,
	[FundingModel] [int] NULL,
    [DelLocPostCode] [VARCHAR(50)] NULL,
    [LearnActEndDate] [Date] NULL,
    [WithdrawReason] [int] NULL,
    [Outcome] [int]	NULL,
    [AimType] [int]	NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
