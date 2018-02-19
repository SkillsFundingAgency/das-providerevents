IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataLock') 
	EXEC('CREATE SCHEMA DataLock')

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Valid') 
	EXEC('CREATE SCHEMA Valid')

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Reference') 
	EXEC('CREATE SCHEMA Reference')

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Rulebase') 
	EXEC('CREATE SCHEMA Rulebase')

IF OBJECT_ID('DataLock.PriceEpisodeMatch', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [DataLock].[PriceEpisodeMatch]) 
		DROP TABLE [DataLock].[PriceEpisodeMatch];
IF OBJECT_ID('DataLock.PriceEpisodePeriodMatch', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [DataLock].[PriceEpisodePeriodMatch]) 
		DROP TABLE [DataLock].[PriceEpisodePeriodMatch];
IF OBJECT_ID('DataLock.ValidationError', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM DataLock.ValidationError) 
		DROP TABLE DataLock.ValidationError; 
IF OBJECT_ID('Valid.LearningProvider', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [Valid].[LearningProvider]) 
		DROP TABLE [Valid].[LearningProvider]; 
IF OBJECT_ID('Valid.Learner', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [Valid].[Learner]) 
		DROP TABLE [Valid].[Learner]; 
IF OBJECT_ID('[Valid].[LearningDelivery]', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [Valid].[LearningDelivery]) 
		DROP TABLE [Valid].[LearningDelivery]; 
IF OBJECT_ID('[dbo].[FileDetails]', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [dbo].[FileDetails]) 
		DROP TABLE [dbo].[FileDetails]; 
IF OBJECT_ID('Reference.DataLockPriceEpisode', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM Reference.DataLockPriceEpisode) 
		DROP TABLE Reference.DataLockPriceEpisode; 
IF OBJECT_ID('dbo.DasCommitments', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM dbo.DasCommitments) 
		DROP TABLE dbo.DasCommitments; 
IF OBJECT_ID('[Rulebase].[AEC_ApprenticeshipPriceEpisode]', 'U') IS NOT NULL 
	IF EXISTS(SELECT 1 FROM [Rulebase].[AEC_ApprenticeshipPriceEpisode]) 
		DROP TABLE [Rulebase].[AEC_ApprenticeshipPriceEpisode]; 

IF OBJECT_ID('DataLock.PriceEpisodeMatch', 'U') IS NULL
    CREATE TABLE DataLock.PriceEpisodeMatch
    (
        [Ukprn] bigint NOT NULL,
        [PriceEpisodeIdentifier] varchar(25) NOT NULL,
        [LearnRefNumber] varchar(100) NOT NULL,
        [AimSeqNumber] bigint NOT NULL,
        [CommitmentId] varchar(50) NOT NULL,
        [CollectionPeriodName] varchar(8),
        [CollectionPeriodMonth] int,
        [CollectionPeriodYear] int,
	    [IsSuccess] bit
    );

IF OBJECT_ID('DataLock.PriceEpisodePeriodMatch', 'U') IS NULL
    CREATE TABLE DataLock.PriceEpisodePeriodMatch
    (
        [Ukprn] bigint NOT NULL,
        [PriceEpisodeIdentifier] varchar(25) NOT NULL,
        [LearnRefNumber] varchar(12) NOT NULL,
        [AimSeqNumber] int NOT NULL,
        [CommitmentId] bigint NOT NULL,
        [VersionId] varchar(25) NOT NULL,
        [Period] int NOT NULL,
        [Payable] bit NOT NULL,
	    [TransactionType] int NOT NULL,
	    [TransactionTypesFlag] int  NULL
    );

IF OBJECT_ID('DataLock.ValidationError', 'U') IS NULL
    CREATE TABLE DataLock.ValidationError
    (
        [Ukprn] bigint,
        [LearnRefNumber] varchar(12),
        [AimSeqNumber] int,
        [RuleId] varchar(50),
        [PriceEpisodeIdentifier] varchar(25) NOT NULL
    );

IF OBJECT_ID('Valid.LearningProvider', 'U') IS NULL
    create table [Valid].[LearningProvider] (
	    [UKPRN] [bigint] NOT NULL PRIMARY KEY
    );

IF OBJECT_ID('[Valid].[Learner]', 'U') IS NULL
	create table [Valid].[Learner](
		[UKPRN] [bigint] NOT NULL,
		[LearnRefNumber] [varchar](12) NOT NULL,
		[PrevLearnRefNumber] [varchar](12) NULL,
		[PrevUKPRN] [bigint] NULL,
		[ULN] [bigint] NOT NULL,
		[FamilyName] [varchar](100) NULL,
		[GivenNames] [varchar](100) NULL,
		[DateOfBirth] [date] NULL,
		[Ethnicity] [int] NOT NULL,
		[Sex] [varchar](1) NOT NULL,
		[LLDDHealthProb] [int] NOT NULL,
		[NINumber] [varchar](9) NULL,
		[PriorAttain] [int] NULL,
		[Accom] [int] NULL,
		[ALSCost] [int] NULL,
		[PlanLearnHours] [int] NULL,
		[PlanEEPHours] [int] NULL,
		[MathGrade] [varchar](4) NULL,
		[EngGrade] [varchar](4) NULL,
		[HomePostcode] [varchar](8) NULL,
		[CurrentPostcode] [varchar](8) NULL,
		[LrnFAM_DLA] [int] NULL,
		[LrnFAM_ECF] [int] NULL,
		[LrnFAM_EDF1] [int] NULL,
		[LrnFAM_EDF2] [int] NULL,
		[LrnFAM_EHC] [int] NULL,
		[LrnFAM_FME] [int] NULL,
		[LrnFAM_HNS] [int] NULL,
		[LrnFAM_LDA] [int] NULL,
		[LrnFAM_LSR1] [int] NULL,
		[LrnFAM_LSR2] [int] NULL,
		[LrnFAM_LSR3] [int] NULL,
		[LrnFAM_LSR4] [int] NULL,
		[LrnFAM_MCF] [int] NULL,
		[LrnFAM_NLM1] [int] NULL,
		[LrnFAM_NLM2] [int] NULL,
		[LrnFAM_PPE1] [int] NULL,
		[LrnFAM_PPE2] [int] NULL,
		[LrnFAM_SEN] [int] NULL,
		[ProvSpecMon_A] [varchar](20) NULL,
		[ProvSpecMon_B] [varchar](20) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[UKPRN] ASC,
		[LearnRefNumber] ASC
	)
	)

IF OBJECT_ID('[Valid].[LearningDelivery]', 'U') IS NULL
	create table [Valid].[LearningDelivery](
		[UKPRN] [bigint] NOT NULL,
		[LearnRefNumber] [varchar](12) NOT NULL,
		[LearnAimRef] [varchar](8) NOT NULL,
		[AimType] [int] NOT NULL,
		[AimSeqNumber] [int] NOT NULL,
		[LearnStartDate] [date] NOT NULL,
		[OrigLearnStartDate] [date] NULL,
		[LearnPlanEndDate] [date] NOT NULL,
		[FundModel] [int] NOT NULL,
		[ProgType] [int] NULL,
		[FworkCode] [int] NULL,
		[PwayCode] [int] NULL,
		[StdCode] [bigint] NULL,
		[PartnerUKPRN] [bigint] NULL,
		[DelLocPostCode] [varchar](8) NULL,
		[AddHours] [int] NULL,
		[PriorLearnFundAdj] [int] NULL,
		[OtherFundAdj] [int] NULL,
		[ConRefNumber] [varchar](20) NULL,
		[EmpOutcome] [int] NULL,
		[CompStatus] [int] NULL,
		[LearnActEndDate] [date] NULL,
		[WithdrawReason] [int] NULL,
		[Outcome] [int] NULL,
		[AchDate] [date] NULL,
		[OutGrade] [varchar](6) NULL,
		[SWSupAimId] [varchar](36) NULL,
		[LrnDelFAM_ADL] [varchar](5) NULL,
		[LrnDelFAM_ASL] [varchar](5) NULL,
		[LrnDelFAM_EEF] [varchar](5) NULL,
		[LrnDelFAM_FFI] [varchar](5) NULL,
		[LrnDelFAM_FLN] [varchar](5) NULL,
		[LrnDelFAM_HEM1] [varchar](5) NULL,
		[LrnDelFAM_HEM2] [varchar](5) NULL,
		[LrnDelFAM_HEM3] [varchar](5) NULL,
		[LrnDelFAM_HHS1] [varchar](5) NULL,
		[LrnDelFAM_HHS2] [varchar](5) NULL,
		[LrnDelFAM_LDM1] [varchar](5) NULL,
		[LrnDelFAM_LDM2] [varchar](5) NULL,
		[LrnDelFAM_LDM3] [varchar](5) NULL,
		[LrnDelFAM_LDM4] [varchar](5) NULL,
		[LrnDelFAM_NSA] [varchar](5) NULL,
		[LrnDelFAM_POD] [varchar](5) NULL,
		[LrnDelFAM_RES] [varchar](5) NULL,
		[LrnDelFAM_SOF] [varchar](5) NULL,
		[LrnDelFAM_SPP] [varchar](5) NULL,
		[LrnDelFAM_WPP] [varchar](5) NULL,
		[ProvSpecMon_A] [varchar](20) NULL,
		[ProvSpecMon_B] [varchar](20) NULL,
		[ProvSpecMon_C] [varchar](20) NULL,
		[ProvSpecMon_D] [varchar](20) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[UKPRN] ASC,
		[LearnRefNumber] ASC,
		[AimSeqNumber] ASC
	)
	)


IF OBJECT_ID('dbo.FileDetails', 'U') IS NULL
    CREATE TABLE [dbo].[FileDetails] (
        [ID] [int] IDENTITY(1,1) PRIMARY KEY,
        [UKPRN] [bigint] NOT NULL,
        [Filename] [nvarchar](50) NULL,
        [FileSizeKb] [bigint] NULL,
        [TotalLearnersSubmitted] [int] NULL,
        [TotalValidLearnersSubmitted] [int] NULL,
        [TotalInvalidLearnersSubmitted] [int] NULL,
        [TotalErrorCount] [int] NULL,
        [TotalWarningCount] [int] NULL,
        [SubmittedTime] [datetime] NULL,
        [Success] [bit]
        CONSTRAINT [PK_dbo.FileDetails] UNIQUE ([UKPRN], [Filename], [Success] ASC)
    );

IF OBJECT_ID('Reference.DataLockPriceEpisode', 'U') IS NULL
	CREATE TABLE Reference.DataLockPriceEpisode (
		[Ukprn] bigint NOT NULL,
		[LearnRefNumber] varchar(12) NOT NULL,
		[Uln] bigint NOT NULL,
		[NiNumber] varchar(9) NULL,
		[AimSeqNumber] int NOT NULL,
		[StandardCode] bigint NULL,
		[ProgrammeType] int NULL,
		[FrameworkCode] int NULL,
		[PathwayCode] int NULL,
		[StartDate] date NOT NULL,
		[NegotiatedPrice] int NOT NULL,
		[PriceEpisodeIdentifier] varchar(25) NOT NULL,
		[EndDate] date NOT NULL,
		[PriceEpisodeFirstAdditionalPaymentThresholdDate] date NULL,
		[PriceEpisodeSecondAdditionalPaymentThresholdDate] date NULL,
		[Tnp1] decimal(15,5) NULL,
		[Tnp2] decimal(15,5) NULL,
		[Tnp3] decimal(15,5) NULL,
		[Tnp4] decimal(15,5) NULL,
		[LearningStartDate] date NULL,
		[EffectiveToDate] date NULL
	)

IF OBJECT_ID('dbo.DasCommitments', 'U') IS NULL
	CREATE TABLE [dbo].[DasCommitments](
		[CommitmentId] [bigint] NOT NULL,
		[VersionId] [varchar](25) NOT NULL,
		[Uln] [bigint] NOT NULL,
		[Ukprn] [bigint] NOT NULL,
		[AccountId] [bigint] NOT NULL,
		[StartDate] [date] NOT NULL,
		[EndDate] [date] NOT NULL,
		[AgreedCost] [decimal](15, 2) NOT NULL,
		[StandardCode] [bigint] NULL,
		[ProgrammeType] [int] NULL,
		[FrameworkCode] [int] NULL,
		[PathwayCode] [int] NULL,
		[PaymentStatus] [int] NOT NULL,
		[PaymentStatusDescription] [varchar](50) NOT NULL,
		[Priority] [int] NOT NULL,
		[EffectiveFromDate] [date] NOT NULL,
		[EffectiveToDate] [date] NULL,
		[LegalEntityName] [nvarchar](100) NULL
	)

IF OBJECT_ID('[Rulebase].[AEC_ApprenticeshipPriceEpisode]', 'U') IS NULL
	CREATE TABLE [Rulebase].[AEC_ApprenticeshipPriceEpisode]
	(
		[Ukprn] bigint NOT NULL,
		[LearnRefNumber] varchar(12),
		[PriceEpisodeIdentifier] varchar(25),
		[EpisodeEffectiveTNPStartDate] date,
		[EpisodeStartDate] date,
		[PriceEpisodeActualEndDate] date,
		[PriceEpisodeActualInstalments] int,
		[PriceEpisodeAimSeqNumber] int,
		[PriceEpisodeCappedRemainingTNPAmount] decimal(10,5),
		[PriceEpisodeCompleted] bit,
		[PriceEpisodeCompletionElement] decimal(10,5),
		[PriceEpisodeExpectedTotalMonthlyValue] decimal(10,5),
		[PriceEpisodeInstalmentValue] decimal(10,5),
		[PriceEpisodePlannedEndDate] date,
		[PriceEpisodePlannedInstalments] int,
		[PriceEpisodePreviousEarnings] decimal(10,5),
		[PriceEpisodeRemainingAmountWithinUpperLimit] decimal(10,5),
		[PriceEpisodeRemainingTNPAmount] decimal(10,5),
		[PriceEpisodeTotalEarnings] decimal(10,5),
		[PriceEpisodeTotalTNPPrice] decimal(10,5),
		[PriceEpisodeUpperBandLimit] decimal(10,5),
		[PriceEpisodeUpperLimitAdjustment] decimal(10,5),
		[TNP1] decimal(10,5),
		[TNP2] decimal(10,5),
		[TNP3] decimal(10,5),
		[TNP4] decimal(10,5),
		PriceEpisodeFirstAdditionalPaymentThresholdDate date NULL,
		PriceEpisodeSecondAdditionalPaymentThresholdDate date NULL,
		PriceEpisodeContractType varchar(50) NULL
		primary key clustered
		(
			[Ukprn] asc,
			[LearnRefNumber] asc,
			[PriceEpisodeIdentifier] asc
		)
	)
