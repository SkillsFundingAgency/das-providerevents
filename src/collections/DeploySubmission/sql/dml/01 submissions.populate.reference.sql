DECLARE @LastProcessedDate datetime = (SELECT MAX(SubmittedTime) FROM ${DAS_ProviderEvents.FQ}.Submissions.LatestVersion)
DECLARE @ProvidersToProcess TABLE (UKPRN bigint)


INSERT INTO @ProvidersToProcess
(UKPRN)
SELECT
	p.UKPRN
FROM ${ILR_Deds.FQ}.Valid.LearningProvider p
INNER JOIN ${ILR_Deds.FQ}.dbo.FileDetails fd
	ON p.UKPRN = fd.UKPRN
	AND fd.SubmittedTime > @LastProcessedDate


INSERT INTO [Reference].[Providers]
(UKPRN)
SELECT
	p.UKPRN,
	fd.Filename,
	fd.SubmittedTime
FROM @ProvidersToProcess p
INNER JOIN ${ILR_Deds.FQ}.dbo.FileDetails fd
	ON p.UKPRN = fd.UKPRN


INSERT INTO [Reference].[LearningDeliveries]
(UKPRN, LearnRefNumber, AimSeqNumber, ULN, NINumber, ProgType, FworkCode, PwayCode, StdCode, LearnStartDate, LearnPlanEndDate, LearnActEndDate, CommitmentId)
SELECT
	ld.UKPRN,
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
	ld.LearnActEndDate,
	pem.CommitmentId
FROM ${ILR_Deds.FQ}.Valid.LearningDelivery ld
INNER JOIN @ProvidersToProcess p
	ON ld.UKPRN = p.UKPRN
INNER JOIN ${ILR_Deds.FQ}.Valid.Learner l
	ON ld.UKPRN = l.UKPRN
	AND ld.LearnRefNumber = l.LearnRefNumber
LEFT JOIN ${ILR_Deds.FQ}.DataLock.PriceEpisodeMatch pem
	ON  ld.Ukprn = pem.Ukprn
    AND pe.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
    AND ld.LearnRefNumber = pem.LearnRefNumber
    AND ld.AimSeqNumber = pem.AimSeqNumber


	

INSERT INTO [Reference].[PriceEpisodes]
(PriceEpisodeIdentifier,Ukprn,LearnRefNumber,PriceEpisodeAimSeqNumber,EpisodeEffectiveTNPStartDate,TNP1,NP2,TNP3,TNP4,CommitmentId,EmpId)


SELECT
	
	pe.PriceEpisodeIdentifier,
	pe.UKPRN,
	pe.LearnRefNumber,
	pe.PriceEpisodeAimSeqNumber,
	pe.EpisodeEffectiveTNPStartDate,
	pe.TNP1,
	pe.TNP2,
	pe.TNP3,
	pe.TNP4,
	(Select Max(CommitmentId) FROM
		 ${ILR_Deds.FQ}.DataLock.PriceEpisodeMatch pem
		WHERE  pe.Ukprn = pem.Ukprn  AND 
				pe.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier AND 
				pe.LearnRefNumber = pem.LearnRefNumber AND 
				pe.PriceEpisodeAimSeqNumber = pem.AimSeqNumber) AS CommitmentId,
	empStatId.EmpId
	
FROM  ${ILR_Deds.FQ}.[Rulebase].[AEC_ApprenticeshipPriceEpisode] pe
JOIN @ProvidersToProcess p
	ON p.UKPRN = pe.UKPRN
JOIN (SELECT Max(empStat.DateEmpStatApp) AS DateEmpStatApp , empStat.UKPRN,empStat.LearnRefNumber
		FROM  ${ILR_Deds.FQ}.[Valid].[LearnerEmploymentStatus] empStat
		JOIN   ${ILR_Deds.FQ}.[Rulebase].[AEC_ApprenticeshipPriceEpisode] pe 	
		 ON empStat.UKPRN = pe.UKPRN 
		AND empStat.LearnRefNumber = pe.LearnRefNumber 	
		WHERE pe.EpisodeEffectiveTNPStartDate >= empStat.DateEmpStatApp AND empStat.UKPRN = pe.UKPRN AND pe.LearnRefNumber = empStat.LearnRefNumber
		GROUP BY empStat.UKPRN,empStat.LearnRefNumber) es
ON 	es.UKPRN  = pe.UKPRN 
AND es.LearnRefNumber = pe.LearnRefNumber

JOIN   ${ILR_Deds.FQ}.[Valid].[LearnerEmploymentStatus] empStatId  
ON 	empStatId.UKPRN  = es.UKPRN 
AND empStatId.LearnRefNumber = es.LearnRefNumber
AND empStatId.DateEmpStatApp = es.DateEmpStatApp


