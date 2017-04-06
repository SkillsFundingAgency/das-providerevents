IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE [name] = 'Submissions')
	BEGIN
		EXEC('CREATE SCHEMA Submissions')
	END
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- vw_Providers
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_Providers' AND [schema_id] = SCHEMA_ID('Submissions'))
BEGIN
    DROP VIEW Submissions.vw_Providers
END
GO

CREATE VIEW Submissions.vw_Providers
AS
SELECT
	fd.UKPRN,
	fd.Filename AS IlrFilename,
	fd.SubmittedTime
FROM dbo.FileDetails fd
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- vw_LearningDeliveries
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_LearningDeliveries' AND [schema_id] = SCHEMA_ID('Submissions'))
BEGIN
    DROP VIEW Submissions.vw_LearningDeliveries
END
GO

CREATE VIEW Submissions.vw_LearningDeliveries
AS
SELECT
	(SELECT UKPRN FROM Valid.LearningProvider) AS UKPRN,
	ld.LearnRefNumber,
	ld.AimSeqNumber,
	l.ULN,
	l.NINumber,
	ld.ProgType,
	ld.FworkCode,
	ld.PwayCode,
	ld.StdCode,
	ld.LearnStartDate,
	ld.LearnPlanEndDate,
	ld.LearnActEndDate
FROM Valid.LearningDelivery ld
INNER JOIN Valid.Learner l
	ON ld.LearnRefNumber = l.LearnRefNumber
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- vw_PriceEpisodes
-----------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS(SELECT [object_id] FROM sys.views WHERE [name]='vw_PriceEpisodes' AND [schema_id] = SCHEMA_ID('Submissions'))
BEGIN
    DROP VIEW Submissions.vw_PriceEpisodes
END
GO

CREATE VIEW Submissions.vw_PriceEpisodes
AS
SELECT
	pe.PriceEpisodeIdentifier,
	(SELECT UKPRN FROM Valid.LearningProvider) AS UKPRN,
	pe.LearnRefNumber,
	pe.PriceEpisodeAimSeqNumber,
	pe.EpisodeEffectiveTNPStartDate,
	pe.TNP1,
	pe.TNP2,
	pe.TNP3,
	pe.TNP4,
	(Select MAX(CommitmentId) FROM DataLock.PriceEpisodeMatch pem
		WHERE pe.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier AND 
			pe.LearnRefNumber = pem.LearnRefNumber AND 
			pe.PriceEpisodeAimSeqNumber = pem.AimSeqNumber) AS CommitmentId,
	empStatId.EmpId
FROM  Rulebase.AEC_ApprenticeshipPriceEpisode pe
	JOIN (SELECT
				MAX(empStat.DateEmpStatApp) AS DateEmpStatApp,
				empStat.LearnRefNumber
			FROM Valid.LearnerEmploymentStatus empStat
			JOIN Rulebase.AEC_ApprenticeshipPriceEpisode pe ON empStat.LearnRefNumber = pe.LearnRefNumber 	
			WHERE pe.EpisodeEffectiveTNPStartDate >= empStat.DateEmpStatApp
				AND pe.LearnRefNumber = empStat.LearnRefNumber
			GROUP BY empStat.LearnRefNumber) es
		ON es.LearnRefNumber = pe.LearnRefNumber
	JOIN Valid.LearnerEmploymentStatus empStatId ON empStatId.LearnRefNumber = es.LearnRefNumber
		AND empStatId.DateEmpStatApp = es.DateEmpStatApp
GO

-----------------------------------------------------------------------------------------------------------------------------------------------
-- CurrentVersion
-----------------------------------------------------------------------------------------------------------------------------------------------
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
	ld.NINumber,
	pe.CommitmentId,
	pe.EmpId As EmployerReferenceNumber
FROM Submissions.vw_Providers p
INNER JOIN Submissions.vw_LearningDeliveries ld
	ON p.UKPRN = ld.UKPRN
INNER JOIN Submissions.vw_PriceEpisodes pe
	ON ld.UKPRN = pe.Ukprn
	AND ld.LearnRefNumber = pe.LearnRefNumber
	AND ld.AimSeqNumber = pe.PriceEpisodeAimSeqNumber


GO
	