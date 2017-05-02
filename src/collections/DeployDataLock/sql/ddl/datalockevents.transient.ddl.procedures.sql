IF EXISTS (SELECT 1 FROM sys.procedures WHERE [name]='GetCurrentEventData' AND [schema_id] =SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP PROCEDURE DataLockEvents.GetCurrentEventData
	END
GO

CREATE PROCEDURE DataLockEvents.GetCurrentEventData
	@UKPRN bigint
AS
SET NOCOUNT ON

	SELECT
		pem.Ukprn,
		pem.PriceEpisodeIdentifier,
		pem.LearnRefNumber,
		pem.AimSeqNumber,
		pem.CommitmentId,
		pem.IsSuccess,

		ilr.IlrFilename,
		ilr.SubmittedTime,
		ilr.Uln,
		ilr.IlrStartDate,
		ilr.IlrStandardCode,
		ilr.IlrProgrammeType,
		ilr.IlrFrameworkCode,
		ilr.IlrPathwayCode,
		ilr.IlrTrainingPrice,
		ilr.IlrEndpointAssessorPrice,
		ilr.IlrPriceEffectiveDate,

		pepm.VersionId CommitmentVersionId,
		pepm.Period,
		pepm.Payable,
		pepm.TransactionType,

		c.AccountId  EmployerAccountId,
		c.StartDate CommitmentStartDate,
		c.StandardCode CommitmentStandardCode,
		c.ProgrammeType CommitmentProgrammeType,
		c.FrameworkCode CommitmentFrameworkCode,
		c.PathwayCode CommitmentPathwayCode,
		c.AgreedCost AS CommitmentNegotiatedPrice,
		c.EffectiveFrom AS CommitmentEffectiveDate,

		err.RuleId
	FROM DataLock.PriceEpisodeMatch pem
	JOIN DataLockEvents.vw_IlrPriceEpisodes ilr
		ON pem.Ukprn = ilr.Ukprn
		AND pem.LearnRefNumber = ilr.LearnRefnumber
		AND pem.PriceEpisodeIdentifier = ilr.PriceEpisodeIdentifier
	JOIN DataLockEvents.vw_PriceEpisodePeriodMatch pepm
		ON pem.Ukprn = pepm.Ukprn
		AND pem.LearnRefNumber = pepm.LearnRefnumber
		AND pem.PriceEpisodeIdentifier = pepm.PriceEpisodeIdentifier
	JOIN Reference.DasCommitments c
		ON pem.CommitmentId = c.CommitmentId
		AND pepm.VersionId = c.VersionId
	LEFT JOIN DataLock.ValidationError err
		ON pem.Ukprn = err.Ukprn
		AND pem.LearnRefNumber = err.LearnRefNumber
		AND pem.PriceEpisodeIdentifier = err.PriceEpisodeIdentifier
	WHERE pem.Ukprn = @UKPRN
	ORDER BY pem.LearnRefNumber, pem.AimSeqNumber, err.RuleId
GO