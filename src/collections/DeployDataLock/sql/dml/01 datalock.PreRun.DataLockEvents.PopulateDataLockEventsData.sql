TRUNCATE TABLE DataLockEvents.DataLockEventsData
GO

INSERT INTO DataLockEvents.DataLockEventsData
(
	Ukprn ,PriceEpisodeIdentifier ,LearnRefNumber ,AimSeqNumber ,CommitmentId ,IsSuccess ,
	IlrFilename,SubmittedTime,ULN,IlrStartDate,IlrStandardCode,IlrProgrammeType,IlrFrameworkCode,IlrPathwayCode,IlrTrainingPrice,IlrEndpointAssessorPrice,IlrPriceEffectiveFromDate,
	CommitmentVersionId ,Period ,Payable,TransactionType ,EmployerAccountId ,CommitmentStartDate,CommitmentStandardCode ,CommitmentProgrammeType,
	CommitmentFrameworkCode,CommitmentPathwayCode ,CommitmentNegotiatedPrice ,CommitmentEffectiveDate ,RuleId 
)

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
		ilr.IlrPriceEffectiveFromDate,

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