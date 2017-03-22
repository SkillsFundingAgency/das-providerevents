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

if object_id('[Valid].[Learner]','u') is not null
	drop table [Valid].[Learner]
create table [Valid].[Learner](
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
PRIMARY KEY CLUSTERED 
(
	[LearnRefNumber] ASC
)
)
GO

if object_id('[Valid].[LearningProvider]','u') is not null
	drop table [Valid].[LearningProvider]
create table [Valid].[LearningProvider](
	[UKPRN] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UKPRN] ASC
)
)
GO

if object_id('[Valid].[LearningDelivery]','u') is not null
	drop table [Valid].[LearningDelivery]
create table [Valid].[LearningDelivery](
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
PRIMARY KEY CLUSTERED 
(
	[LearnRefNumber] ASC,
	[AimSeqNumber] ASC
)
)
GO

if object_id('[Valid].[LearnerEmploymentStatus]','u') is not null
	drop table [Valid].[LearnerEmploymentStatus]
create table [Valid].[LearnerEmploymentStatus](
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
	[LearnRefNumber] ASC,
	[DateEmpStatApp] ASC
)
)
GO

IF OBJECT_ID('FileDetails') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[FileDetails]
END
GO

CREATE TABLE [dbo].[FileDetails](
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
 CONSTRAINT [PK_dbo.FileDetails] UNIQUE
(
	[UKPRN],[Filename],[Success] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='AEC_ApprenticeshipPriceEpisode' AND [schema_id] = SCHEMA_ID('Rulebase'))
BEGIN
	DROP TABLE Rulebase.AEC_ApprenticeshipPriceEpisode
END
GO

CREATE TABLE [Rulebase].[AEC_ApprenticeshipPriceEpisode]
(
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
		[LearnRefNumber] asc,
		[PriceEpisodeIdentifier] asc
	)
)
GO