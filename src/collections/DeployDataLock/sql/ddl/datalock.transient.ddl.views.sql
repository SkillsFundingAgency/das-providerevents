IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_PriceEpisodeMatchCurrentVersion' AND [schema_id] = SCHEMA_ID('DataLock'))
BEGIN
    DROP VIEW DataLock.vw_PriceEpisodeMatchCurrentVersion
END
GO

CREATE VIEW DataLock.vw_PriceEpisodeMatchCurrentVersion
AS
SELECT
    p.IlrFilename,
    p.SubmittedTime,
    p.Ukprn,
    iped.Uln,
    iped.LearnRefNumber,
    iped.AimSeqNumber,
    iped.PriceEpisodeIdentifier,
	pem.CommitmentId,
    (SELECT TOP 1 EmployerAccountId FROM Reference.DataLockCommitmentData WHERE CommitmentId = pem.CommitmentId) EmployerAccountId,
    pem.IsSuccess,
    iped.StartDate IlrStartDate,
    iped.StdCode IlrStandardCode,
    iped.ProgType IlrProgrammeType,
    iped.FworkCode IlrFrameworkCode,
    iped.PwayCode IlrPathwayCode,
	CASE
		WHEN ISNULL(iped.TNP1, 0) > 0 THEN iped.TNP1
		ELSE iped.TNP3
	END IlrTrainingPrice,
	CASE
		WHEN ISNULL(iped.TNP1, 0) > 0 THEN iped.TNP2
		ELSE iped.TNP4
	END IlrEndpointAssessorPrice
FROM Reference.PriceEpisodeMatch pem
INNER JOIN Reference.Providers p
    ON pem.Ukprn = p.Ukprn
INNER JOIN Reference.IlrPriceEpisodeData iped
	ON pem.Ukprn = iped.Ukprn
    AND pem.PriceEpisodeIdentifier = iped.PriceEpisodeIdentifier
	AND pem.LearnRefNumber = iped.LearnRefNumber
