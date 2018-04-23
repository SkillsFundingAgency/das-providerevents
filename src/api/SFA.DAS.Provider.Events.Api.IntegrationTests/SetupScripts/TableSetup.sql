-------------------------------------------------------------
---		SCHEMAS
-------------------------------------------------------------
IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'DataLock')
	BEGIN
		EXEC('CREATE SCHEMA DataLock')
	END
GO

IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Payments')
	BEGIN
		EXEC('CREATE SCHEMA Payments')
	END
GO

IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'PaymentsDue')
	BEGIN
		EXEC('CREATE SCHEMA PaymentsDue')
	END
GO

IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'AccountTransfers')
	BEGIN
		EXEC('CREATE SCHEMA AccountTransfers')
	END
GO


-------------------------------------------------------------
---		DataLock.DataLockEvents
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEvents' AND [schema_id] = SCHEMA_ID('DataLock'))
	BEGIN
		DROP TABLE [DataLock].[DataLockEvents]
	END
GO
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
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-------------------------------------------------------------
---		DataLock.DataLockEventPeriods
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventPeriods' AND [schema_id] = SCHEMA_ID('DataLock'))
	BEGIN
		DROP TABLE [DataLock].[DataLockEventPeriods]
	END
GO
CREATE TABLE [DataLock].[DataLockEventPeriods](
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[CommitmentVersion] [varchar](25) NOT NULL,
	[IsPayable] [bit] NOT NULL,
	[TransactionType] [int] NOT NULL
) ON [PRIMARY]
GO


-------------------------------------------------------------
---		DataLock.DataLockEventErrors
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventErrors' AND [schema_id] = SCHEMA_ID('DataLock'))
	BEGIN
		DROP TABLE [DataLock].[DataLockEventErrors]
	END
GO
CREATE TABLE [DataLock].[DataLockEventErrors](
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[ErrorCode] [varchar](15) NOT NULL,
	[SystemDescription] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DataLockEventId] ASC,
	[ErrorCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-------------------------------------------------------------
---		DataLock.DataLockEventCommitmentVersion
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'DataLockEventCommitmentVersions' AND [schema_id] = SCHEMA_ID('DataLock'))
	BEGIN
		DROP TABLE [DataLock].[DataLockEventCommitmentVersions]
	END
GO
CREATE TABLE [DataLock].[DataLockEventCommitmentVersions](
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[CommitmentVersion] [varchar](25) NOT NULL,
	[CommitmentStartDate] [date] NOT NULL,
	[CommitmentStandardCode] [bigint] NULL,
	[CommitmentProgrammeType] [int] NULL,
	[CommitmentFrameworkCode] [int] NULL,
	[CommitmentPathwayCode] [int] NULL,
	[CommitmentNegotiatedPrice] [decimal](12, 5) NOT NULL,
	[CommitmentEffectiveDate] [date] NOT NULL
) ON [PRIMARY]
GO


-------------------------------------------------------------
---		Payments.Payments
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'Payments' AND [schema_id] = SCHEMA_ID('Payments'))
	BEGIN
		DROP TABLE [Payments].[Payments]
	END
GO
CREATE TABLE [Payments].[Payments](
	[PaymentId] [uniqueidentifier] NOT NULL,
	[RequiredPaymentId] [uniqueidentifier] NOT NULL,
	[DeliveryMonth] [int] NOT NULL,
	[DeliveryYear] [int] NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[FundingSource] [int] NOT NULL,
	[FundingAccountId] [bigint] NULL,
	[TransactionType] [int] NOT NULL,
	[Amount] [decimal](15, 5) NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Payments].[Payments] ADD  DEFAULT (newid()) FOR [PaymentId]
GO


-------------------------------------------------------------
---		Payments.Periods
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'Periods' AND [schema_id] = SCHEMA_ID('Payments'))
	BEGIN
		DROP TABLE [Payments].[Periods]
	END
GO
CREATE TABLE [Payments].[Periods](
	[PeriodName] [char](8) NOT NULL,
	[CalendarMonth] [int] NOT NULL,
	[CalendarYear] [int] NOT NULL,
	[AccountDataValidAt] [datetime] NULL,
	[CommitmentDataValidAt] [datetime] NULL,
	[CompletionDateTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PeriodName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-------------------------------------------------------------
---		PaymentsDue.RequiredPayment
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'RequiredPayments' AND [schema_id] = SCHEMA_ID('PaymentsDue'))
	BEGIN
		DROP TABLE [PaymentsDue].[RequiredPayments]
	END
GO
CREATE TABLE [PaymentsDue].[RequiredPayments](
	[Id] [uniqueidentifier] NOT NULL,
	[CommitmentId] [bigint] NULL,
	[CommitmentVersionId] [varchar](25) NULL,
	[AccountId] [bigint] NULL,
	[AccountVersionId] [varchar](50) NULL,
	[Uln] [bigint] NULL,
	[LearnRefNumber] [varchar](12) NULL,
	[AimSeqNumber] [int] NULL,
	[Ukprn] [bigint] NULL,
	[IlrSubmissionDateTime] [datetime] NULL,
	[PriceEpisodeIdentifier] [varchar](25) NULL,
	[StandardCode] [bigint] NULL,
	[ProgrammeType] [int] NULL,
	[FrameworkCode] [int] NULL,
	[PathwayCode] [int] NULL,
	[ApprenticeshipContractType] [int] NULL,
	[DeliveryMonth] [int] NULL,
	[DeliveryYear] [int] NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[TransactionType] [int] NULL,
	[AmountDue] [decimal](15, 5) NULL,
	[SfaContributionPercentage] [decimal](15, 5) NULL,
	[FundingLineType] [varchar](100) NULL,
	[UseLevyBalance] [bit] NULL,
	[LearnAimRef] [varchar](8) NULL,
	[LearningStartDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [PaymentsDue].[RequiredPayments] ADD  DEFAULT (newid()) FOR [Id]
GO


-------------------------------------------------------------
---		PaymentsDue.Earnings
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'Earnings' AND [schema_id] = SCHEMA_ID('PaymentsDue'))
	BEGIN
		DROP TABLE [PaymentsDue].[Earnings]
	END
GO
CREATE TABLE [PaymentsDue].[Earnings](
	[RequiredPaymentId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[PlannedEndDate] [datetime] NOT NULL,
	[ActualEndDate] [datetime] NULL,
	[CompletionStatus] [int] NULL,
	[CompletionAmount] [decimal](10, 5) NULL,
	[MonthlyInstallment] [decimal](10, 5) NOT NULL,
	[TotalInstallments] [int] NOT NULL,
	[EndpointAssessorId] varchar(7) NULL
) ON [PRIMARY]
GO

-------------------------------------------------------------
---		TransferPayments.AccountTransfers
-------------------------------------------------------------
IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'TransferPayments' AND [schema_id] = SCHEMA_ID('AccountTransfers'))
	BEGIN
		DROP TABLE [AccountTransfers].[TransferPayments]
	END
GO
IF NOT EXISTS(SELECT NULL FROM 
	sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
	WHERE t.name='TransferPayments' AND s.name='AccountTransfers'
)
BEGIN
	CREATE TABLE [AccountTransfers].[TransferPayments]
	(
		Id uniqueidentifier PRIMARY KEY DEFAULT(NEWID()),
		SendingAccountId bigint NOT NULL,
		RecievingAccountId bigint NOT NULL,
		RequiredPaymentId uniqueidentifier NOT NULL,
		CommitmentId bigint NOT NULL,
		Amount decimal(15,5) NOT NULL,
		TransferType int NOT NULL,
		TransferDate DateTime NOT NULL,
		CollectionPeriodName varchar(8) NOT NULL
	)
END
GO
