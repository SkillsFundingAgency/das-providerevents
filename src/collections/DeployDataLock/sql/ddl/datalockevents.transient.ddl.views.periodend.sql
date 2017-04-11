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
    p.IlrFilename,
    p.IlrSubmissionDateTime SubmittedTime,
    dpe.Ukprn,
    dpe.Uln,
    dpe.PriceEpisodeIdentifier,
    dpe.LearnRefNumber,
    dpe.AimSeqNumber,
    dpe.LearningStartDate IlrStartDate,
    dpe.StandardCode IlrStandardCode,
    dpe.ProgrammeType IlrProgrammeType,
    dpe.FrameworkCode IlrFrameworkCode,
    dpe.PathwayCode IlrPathwayCode,
	CASE
		WHEN ISNULL(dpe.Tnp1, 0) > 0 THEN dpe.Tnp1
		ELSE dpe.Tnp3
	END IlrTrainingPrice,
	CASE
		WHEN ISNULL(dpe.Tnp1, 0) > 0 THEN dpe.Tnp2
		ELSE dpe.Tnp4
	END IlrEndpointAssessorPrice,
	dpe.StartDate IlrPriceEffectiveDate
FROM Reference.DataLockPriceEpisode dpe
	JOIN Reference.Providers p 
		ON dpe.Ukprn = p.Ukprn
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
