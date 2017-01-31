IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='CurrentVersion' AND [schema_id] = SCHEMA_ID('Submissions'))
BEGIN
    DROP VIEW Submissions.CurrentVersion
END
GO

CREATE VIEW Submissions.CurrentVersion
AS
SELECT
	p.IlrFilename,
	Submissions.ExtractDateFromFileName(p.IlrFilename) FileDateTime,
	p.SubmittedTime SubmittedDateTime,
	p.UKPRN,
	ld.ULN,
	ld.LearnRefNumber,
	ld.AimSeqNumber,
	pe.PriceEpisodeIdentifier PriceEpisodeIdentifier,
	ld.StdCode StandardCode,
	ld.ProgType ProgrammeType,
	ld.FworkCode FrameworkCode,
	ld.PwayCode PathwayCode,
	ld.LearnStartDate ActualStartDate,
	ld.LearnPlanEndDate PlannedEndDate,
	ld.LearnActEndDate ActualEndDate,
	CASE
		WHEN ISNULL(pe.TNP1,0) > 0 THEN pe.TNP1
		ELSE pe.TNP3
	END OnProgrammeTotalPrice,
	CASE
		WHEN ISNULL(pe.TNP1,0) > 0 THEN pe.TNP2
		ELSE pe.TNP4
	END CompletionTotalPrice,
	ld.NINumber
FROM Reference.Providers p
INNER JOIN Reference.LearningDeliveries ld
	ON p.UKPRN = ld.UKPRN
INNER JOIN Reference.PriceEdpisodes pe
	ON ld.UKPRN = pe.Ukprn
	AND ld.LearnRefNumber = pe.LearnRefNumber
	AND ld.AimSeqNumber = pe.PriceEpisodeAimSeqNumber