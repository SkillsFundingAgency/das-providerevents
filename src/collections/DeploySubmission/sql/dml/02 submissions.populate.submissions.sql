--------------------------------------------------------------------------------------
-- IdentifierSeed
--------------------------------------------------------------------------------------
INSERT INTO Reference.IdentifierSeed
(IdentifierName, MaxIdInDeds)
SELECT
	'SubmissionEvents',
	MAX(Id)
FROM ${DAS_ProviderEvents.FQ}.Submissions.SubmissionEvents

---------------------------------------------------------------
-- LatestVersion
---------------------------------------------------------------
INSERT INTO [Submissions].[LatestVersion]
(
	IlrFileName,
	FileDateTime,
	SubmittedDateTime,
	ComponentVersionNumber,
	UKPRN,
	ULN,
	LearnRefNumber,
    AimSeqNumber,
	PriceEpisodeIdentifier,
	StandardCode,
	ProgrammeType,
	FrameworkCode,
	PathwayCode,
	ActualStartDate,
	PlannedEndDate,
	ActualEndDate,
	OnProgrammeTotalPrice,
	CompletionTotalPrice,
	NINumber
)
SELECT
	IlrFileName,
	FileDateTime,
	SubmittedDateTime,
	ComponentVersionNumber,
	UKPRN,
	ULN,
	LearnRefNumber,
    AimSeqNumber,
	PriceEpisodeIdentifier,
	StandardCode,
	ProgrammeType,
	FrameworkCode,
	PathwayCode,
	ActualStartDate,
	PlannedEndDate,
	ActualEndDate,
	OnProgrammeTotalPrice,
	CompletionTotalPrice,
	NINumber
FROM ${DAS_ProviderEvents.FQ}.Submissions.LatestVersion lv
WHERE UKPRN IN (SELECT UKPRN FROM [Reference].[Providers]
GO