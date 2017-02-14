IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Reference')
	BEGIN
		EXEC('CREATE SCHEMA Reference')
	END
GO

--------------------------------------------------------------------------------------
-- vw_IlrPriceEpisodes
--------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_IlrPriceEpisodes' AND [schema_id] = SCHEMA_ID('Reference'))
BEGIN
    DROP VIEW Reference.vw_IlrPriceEpisodes
END
GO

CREATE VIEW Reference.vw_IlrPriceEpisodes
AS
SELECT
    p.IlrFilename,
    p.SubmittedTime,
    p.Ukprn,
    pe.Uln,
    pe.PriceEpisodeIdentifier,
    pe.LearnRefNumber,
    pe.AimSeqNumber,
    pe.StartDate IlrStartDate,
    pe.StdCode IlrStandardCode,
    pe.ProgType IlrProgrammeType,
    pe.FworkCode IlrFrameworkCode,
    pe.PwayCode IlrPathwayCode,
	CASE
		WHEN ISNULL(pe.TNP1, 0) > 0 THEN pe.TNP1
		ELSE pe.TNP3
	END IlrTrainingPrice,
	CASE
		WHEN ISNULL(pe.TNP1, 0) > 0 THEN pe.TNP2
		ELSE pe.TNP4
	END IlrEndpointAssessorPrice
FROM Reference.IlrPriceEpisodes pe
INNER JOIN Reference.Providers p
    ON pe.Ukprn = p.Ukprn
GO
