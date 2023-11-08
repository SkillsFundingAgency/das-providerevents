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
	[OnProgrammeTotalPrice] [decimal](15, 5) NULL,
	[CompletionTotalPrice] [decimal](15, 5) NULL,
	[NINumber] [varchar](9) NULL,
	[CommitmentId] [bigint] NULL,
	[AcademicYear] [varchar](4) NOT NULL,
	[EmployerReferenceNumber] [int] NULL,
	[EPAOrgId] [varchar](7) NULL,
	[GivenNames] [varchar](100) NULL,
	[FamilyName] [varchar](100) NULL,
	[CompStatus] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]