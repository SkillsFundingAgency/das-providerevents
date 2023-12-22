CREATE TABLE [DataLock].[DataLockEvents](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[ProcessDateTime] [datetime] NOT NULL,
	[IlrFileName] [nvarchar](50) NOT NULL,
	[SubmittedDateTime] [datetime] NOT NULL,
	[AcademicYear] [varchar](4) NOT NULL,
	[UKPRN] [bigint] NOT NULL,
	[ULN] [bigint] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[AimSeqNumber] [int] NOT NULL,
	[PriceEpisodeIdentifier] [varchar](25) NOT NULL,
	[CommitmentId] [bigint] NOT NULL,
	[EmployerAccountId] [bigint] NOT NULL,
	[EventSource] [int] NOT NULL,
	[HasErrors] [bit] NOT NULL,
	[IlrStartDate] [date] NULL,
	[IlrStandardCode] [bigint] NULL,
	[IlrProgrammeType] [int] NULL,
	[IlrFrameworkCode] [int] NULL,
	[IlrPathwayCode] [int] NULL,
	[IlrTrainingPrice] [decimal](12, 5) NULL,
	[IlrEndpointAssessorPrice] [decimal](12, 5) NULL,
	[IlrPriceEffectiveFromDate] [date] NULL,
	[IlrPriceEffectiveToDate] [date] NULL,
	[Status] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NULL
) ON [PRIMARY]
GO
ALTER TABLE [DataLock].[DataLockEvents] ADD PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DataLockEvents_DataLockId] ON [DataLock].[DataLockEvents]
(
	[DataLockEventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [DataLock].[DataLockEvents] ADD  CONSTRAINT [DF_DataLockEvents__CreationDate]  DEFAULT (sysdatetimeoffset()) FOR [CreationDate]
GO