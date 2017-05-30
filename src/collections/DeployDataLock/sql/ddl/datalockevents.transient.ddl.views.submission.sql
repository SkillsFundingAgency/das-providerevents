IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'DataLockEvents')
	BEGIN
		EXEC('CREATE SCHEMA DataLockEvents')
	END
GO

--------------------------------------------------------------------------------------
-- vw_IlrPriceEpisodes
--------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_IlrPriceEpisodes' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
BEGIN
    DROP VIEW DataLockEvents.vw_IlrPriceEpisodes
END
GO

CREATE VIEW DataLockEvents.vw_IlrPriceEpisodes
AS
SELECT
    fd.Filename IlrFilename,
    fd.SubmittedTime,
    fd.Ukprn,
    l.Uln,
    pe.PriceEpisodeIdentifier,
    pe.LearnRefNumber,
    pe.PriceEpisodeAimSeqNumber AimSeqNumber,
    pe.EpisodeEffectiveTNPStartDate IlrStartDate,
    ld.StdCode IlrStandardCode,
    ld.ProgType IlrProgrammeType,
    ld.FworkCode IlrFrameworkCode,
    ld.PwayCode IlrPathwayCode,
	CASE
		WHEN ISNULL(pe.TNP1, 0) > 0 THEN pe.TNP1
		ELSE pe.TNP3
	END IlrTrainingPrice,
	CASE
		WHEN ISNULL(pe.TNP1, 0) > 0 THEN pe.TNP2
		ELSE pe.TNP4
	END IlrEndpointAssessorPrice,
	pe.EpisodeEffectiveTNPStartDate IlrPriceEffectiveFromDate
FROM Rulebase.AEC_ApprenticeshipPriceEpisode pe
	JOIN Valid.Learner l ON pe.LearnRefNumber = l.LearnRefNumber
	JOIN Valid.LearningDelivery ld ON pe.LearnRefNumber = ld.LearnRefNumber
		AND pe.PriceEpisodeAimSeqNumber = ld.AimSeqNumber
	CROSS JOIN (
		SELECT TOP 1 *
		FROM dbo.FileDetails
	) fd
GO



--------------------------------------------------------------------------------------
-- vw_PriceEpisodePeriodMatch
--------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_PriceEpisodePeriodMatch' AND [schema_id] = SCHEMA_ID('DataLockEvents'))
BEGIN
    DROP VIEW DataLockEvents.vw_PriceEpisodePeriodMatch
END
GO

CREATE VIEW DataLockEvents.vw_PriceEpisodePeriodMatch
AS
SELECT
    pepm.Ukprn, 
	pepm.PriceEpisodeIdentifier, 
	pepm.LearnRefnumber, 
	pepm.AimSeqNumber, 
	pepm.CommitmentId, 
	pepm.VersionId, 
	pepm.Period, 
	CASE WHEN pem.IsSuccess = 1 And pepm.Payable = 1 Then 1 ELSE 0 END As Payable , 
	pepm.TransactionType
	FROM DataLock.PriceEpisodePeriodMatch pepm 
	LEFT JOIN DataLock.PriceEpisodeMatch pem on 
		pem.Ukprn = pepm.Ukprn AND
		pem.LearnRefnumber= pepm.LearnRefnumber AND
		pem.AimSeqNumber = pepm.AimSeqNumber AND
		pem.PriceEpisodeIdentifier = pepm.PriceEpisodeIdentifier 
GO
