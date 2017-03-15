IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='Valid')
BEGIN
	EXEC('CREATE SCHEMA Valid')
END
GO

IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='Rulebase')
BEGIN
	EXEC('CREATE SCHEMA Rulebase')
END
GO



IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='AEC_ApprenticeshipPriceEpisode' AND [schema_id] = SCHEMA_ID('Rulebase'))
BEGIN
       DROP TABLE [Rulebase].[AEC_ApprenticeshipPriceEpisode]
END
GO

CREATE TABLE [Rulebase].[AEC_ApprenticeshipPriceEpisode](
	[Ukprn] [bigint] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[PriceEpisodeIdentifier] [varchar](25) NOT NULL,
	[EpisodeEffectiveTNPStartDate] [date] NULL,
	[EpisodeStartDate] [date] NULL,
	[PriceEpisodeActualEndDate] [date] NULL,
	[PriceEpisodeActualInstalments] [int] NULL,
	[PriceEpisodeAimSeqNumber] [int] NULL,
	[PriceEpisodeCappedRemainingTNPAmount] [decimal](10, 5) NULL,
	[PriceEpisodeCompleted] [bit] NULL,
	[PriceEpisodeCompletionElement] [decimal](10, 5) NULL,
	[PriceEpisodeContractType] [varchar](50) NULL,
	[PriceEpisodeExpectedTotalMonthlyValue] [decimal](10, 5) NULL,
	[PriceEpisodeFirstAdditionalPaymentThresholdDate] [date] NULL,
	[PriceEpisodeFundLineType] [varchar](60) NULL,
	[PriceEpisodeInstalmentValue] [decimal](10, 5) NULL,
	[PriceEpisodePlannedEndDate] [date] NULL,
	[PriceEpisodePlannedInstalments] [int] NULL,
	[PriceEpisodePreviousEarnings] [decimal](10, 5) NULL,
	[PriceEpisodeRemainingAmountWithinUpperLimit] [decimal](10, 5) NULL,
	[PriceEpisodeRemainingTNPAmount] [decimal](10, 5) NULL,
	[PriceEpisodeSecondAdditionalPaymentThresholdDate] [date] NULL,
	[PriceEpisodeTotalEarnings] [decimal](10, 5) NULL,
	[PriceEpisodeTotalTNPPrice] [decimal](10, 5) NULL,
	[PriceEpisodeUpperBandLimit] [decimal](10, 5) NULL,
	[PriceEpisodeUpperLimitAdjustment] [decimal](10, 5) NULL,
	[TNP1] [decimal](10, 5) NULL,
	[TNP2] [decimal](10, 5) NULL,
	[TNP3] [decimal](10, 5) NULL,
	[TNP4] [decimal](10, 5) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Ukprn] ASC,
		[LearnRefNumber] ASC,
		[PriceEpisodeIdentifier] ASC
	)
)
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- LearningProvider
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='LearningProvider' AND [schema_id] = SCHEMA_ID('Valid'))
BEGIN
	DROP TABLE Valid.LearningProvider
END
GO

create table [Valid].[LearningProvider] (
	[UKPRN] [int] NOT NULL,
	PRIMARY KEY CLUSTERED ([UKPRN] ASC)
)

if object_id('[Valid].[Learner]','u') is not null
	drop table [Valid].[Learner]
GO

create table [Valid].[Learner](
	[UKPRN] [int] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[PrevLearnRefNumber] [varchar](12) NULL,
	[PrevUKPRN] [int] NULL,
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
	PRIMARY KEY CLUSTERED ([UKPRN] ASC,	[LearnRefNumber] ASC)
)
GO


if object_id('[Valid].[LearningDelivery]','u') is not null
	drop table [Valid].[LearningDelivery]
GO

create table [Valid].[LearningDelivery](
	[UKPRN] [int] NOT NULL,
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
	[PartnerUKPRN] [int] NULL,
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
	PRIMARY KEY CLUSTERED ([UKPRN] ASC, [LearnRefNumber] ASC, [AimSeqNumber] ASC)
)
GO

if object_id('[Valid].[LearningDeliveryFAM]','u') is not null
	drop table [Valid].[LearningDeliveryFAM]
GO

create table [Valid].[LearningDeliveryFAM](
	[UKPRN] [int] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[AimSeqNumber] [int] NOT NULL,
	[LearnDelFAMType] [varchar](3) NULL,
	[LearnDelFAMCode] [varchar](5) NULL,
	[LearnDelFAMDateFrom] [date] NULL,
	[LearnDelFAMDateTo] [date] NULL
)
GO

create index [IX_Valid_LearningDeliveryFAM] on [Valid].[LearningDeliveryFAM]
	(
		[UKPRN] asc,
		[LearnRefNumber] asc,
		[AimSeqNumber] asc,
		[LearnDelFAMType] asc
	)
GO

if object_id('[Valid].[TrailblazerApprenticeshipFinancialRecord]','u') is not null
	drop table [Valid].[TrailblazerApprenticeshipFinancialRecord]
GO

create table [Valid].[TrailblazerApprenticeshipFinancialRecord](
	[UKPRN] [int] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[AimSeqNumber] [int] NOT NULL,
	[TBFinType] [varchar](3) NOT NULL,
	[TBFinCode] [int] NULL,
	[TBFinDate] [date] NULL,
	[TBFinAmount] [int] NOT NULL
)
GO

create index [IX_Valid_TrailblazerApprenticeshipFinancialRecord] on [Valid].[TrailblazerApprenticeshipFinancialRecord]
	(
		[UKPRN] asc,
		[LearnRefNumber] asc,
		[AimSeqNumber] asc,
		[TBFinType] asc
	)
GO

IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='FileDetails' AND [schema_id] = SCHEMA_ID('dbo'))
BEGIN
       DROP TABLE [dbo].[FileDetails]
END
GO

CREATE TABLE [dbo].[FileDetails] (
    [ID] [int] IDENTITY(1,1),
    [UKPRN] [int] NOT NULL,
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
)
GO


IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='LearnerEmploymentStatus' AND [schema_id] = SCHEMA_ID('Valid'))
BEGIN
       DROP TABLE [Valid].[LearnerEmploymentStatus]
END
GO

CREATE TABLE [Valid].[LearnerEmploymentStatus](
	[UKPRN] [bigint] NOT NULL,
	[LearnRefNumber] [varchar](12) NOT NULL,
	[EmpStat] [int] NULL,
	[DateEmpStatApp] [date] NOT NULL,
	[EmpId] [int] NULL,
	[EmpStatMon_BSI] [int] NULL,
	[EmpStatMon_EII] [int] NULL,
	[EmpStatMon_LOE] [int] NULL,
	[EmpStatMon_LOU] [int] NULL,
	[EmpStatMon_PEI] [int] NULL,
	[EmpStatMon_SEI] [int] NULL,
	[EmpStatMon_SEM] [int] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[UKPRN] ASC,
		[LearnRefNumber] ASC,
		[DateEmpStatApp] ASC
	)
)
GO