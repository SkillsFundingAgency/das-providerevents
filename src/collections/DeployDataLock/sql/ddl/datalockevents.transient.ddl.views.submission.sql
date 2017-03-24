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
	END IlrEndpointAssessorPrice
FROM Rulebase.AEC_ApprenticeshipPriceEpisode pe
	JOIN Valid.Learner l ON pe.LearnRefNumber = l.LearnRefNumber
	JOIN Valid.LearningDelivery ld ON pe.LearnRefNumber = ld.LearnRefNumber
		AND pe.PriceEpisodeAimSeqNumber = ld.AimSeqNumber
	CROSS JOIN (
		SELECT TOP 1 *
		FROM dbo.FileDetails
	) fd
GO
