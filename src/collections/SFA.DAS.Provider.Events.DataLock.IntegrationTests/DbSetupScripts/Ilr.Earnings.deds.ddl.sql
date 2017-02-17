IF NOT EXISTS(SELECT [schema_id] FROM sys.schemas WHERE [name]='Rulebase')
BEGIN
	EXEC('CREATE SCHEMA Rulebase')
END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- AEC_ApprenticeshipPriceEpisode
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.tables WHERE [name]='AEC_ApprenticeshipPriceEpisode' AND [schema_id] = SCHEMA_ID('Rulebase'))
BEGIN
	DROP TABLE Rulebase.AEC_ApprenticeshipPriceEpisode
END
GO

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
	PriceEpisodeSecondAdditionalPaymentThresholdDate date NULL
	primary key clustered
	(
		[Ukprn] asc,
		[LearnRefNumber] asc,
		[PriceEpisodeIdentifier] asc
	)
)
GO
