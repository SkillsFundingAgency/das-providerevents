DECLARE @ProvidersToProcess TABLE (UKPRN bigint)
INSERT INTO @ProvidersToProcess
(UKPRN)
SELECT
	p.UKPRN
FROM Reference.Providers p

DECLARE @PeriodToProcess TABLE (CollectionPeriodName varchar(8), CollectionPeriodMonth int, CollectionPeriodYear int)
INSERT INTO @PeriodToProcess
(CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear)
SELECT TOP 1
	p.PeriodName, p.CalendarMonth, p.CalendarYear
FROM ${DAS_PeriodEnd.FQ}.[Payments].[Periods] p
WHERE PeriodName LIKE '${YearOfCollection}-%'
ORDER BY CompletionDateTime DESC

---------------------------------------------------------------
-- PriceEpisodeMatch
---------------------------------------------------------------
INSERT INTO [Reference].[PriceEpisodeMatch]
(UKPRN, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, CommitmentId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, IsSuccess)
SELECT
	pem.UKPRN, 
	pem.PriceEpisodeIdentifier, 
	pem.LearnRefNumber, 
	pem.AimSeqNumber, 
	pem.CommitmentId, 
	pem.CollectionPeriodName, 
	pem.CollectionPeriodMonth, 
	pem.CollectionPeriodYear, 
	pem.IsSuccess
FROM ${DataLock_Deds.FQ}.DataLock.PriceEpisodeMatch pem
INNER JOIN @ProvidersToProcess p
	ON pem.UKPRN = p.UKPRN
INNER JOIN @PeriodToProcess pp
	ON pem.CollectionPeriodName = pp.CollectionPeriodName
	AND pem.CollectionPeriodMonth = pp.CollectionPeriodMonth
	AND pem.CollectionPeriodYear = pp.CollectionPeriodYear

---------------------------------------------------------------
-- PriceEpisodePeriodMatch
---------------------------------------------------------------
INSERT INTO [Reference].[PriceEpisodePeriodMatch]
(Ukprn, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, CommitmentId, VersionId, Period, Payable, TransactionType, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear)
SELECT
	pepm.Ukprn, 
	pepm.PriceEpisodeIdentifier, 
	pepm.LearnRefNumber, 
	pepm.AimSeqNumber, 
	pepm.CommitmentId, 
	pepm.VersionId, 
	pepm.Period, 
	pepm.Payable, 
	pepm.TransactionType, 
	pepm.CollectionPeriodName, 
	pepm.CollectionPeriodMonth, 
	pepm.CollectionPeriodYear
FROM ${DataLock_Deds.FQ}.DataLock.PriceEpisodePeriodMatch pepm
INNER JOIN @ProvidersToProcess p
	ON pepm.UKPRN = p.UKPRN
INNER JOIN @PeriodToProcess pp
	ON pepm.CollectionPeriodName = pp.CollectionPeriodName
	AND pepm.CollectionPeriodMonth = pp.CollectionPeriodMonth
	AND pepm.CollectionPeriodYear = pp.CollectionPeriodYear

---------------------------------------------------------------
-- ValidationError
---------------------------------------------------------------
INSERT INTO [Reference].[ValidationError]
(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear)
SELECT
	ve.Ukprn, 
	ve.LearnRefNumber, 
	ve.AimSeqNumber, 
	ve.RuleId, 
	ve.PriceEpisodeIdentifier, 
	ve.CollectionPeriodName, 
	ve.CollectionPeriodMonth, 
	ve.CollectionPeriodYear
FROM ${DataLock_Deds.FQ}.DataLock.ValidationError ve
INNER JOIN @ProvidersToProcess p
	ON ve.UKPRN = p.UKPRN
INNER JOIN @PeriodToProcess pp
	ON ve.CollectionPeriodName = pp.CollectionPeriodName
	AND ve.CollectionPeriodMonth = pp.CollectionPeriodMonth
	AND ve.CollectionPeriodYear = pp.CollectionPeriodYear

---------------------------------------------------------------
-- IlrPriceEpisodes
---------------------------------------------------------------
INSERT INTO [Reference].[IlrPriceEpisodes]
(Ukprn, Uln, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, StartDate, ProgType, FworkCode, PwayCode, StdCode, TNP1, TNP2, TNP3, TNP4)
SELECT
	pe.Ukprn, 
    l.Uln, 
    pe.PriceEpisodeIdentifier, 
	pe.LearnRefNumber, 
	pe.PriceEpisodeAimSeqNumber, 
	pe.EpisodeEffectiveTNPStartDate, 
	ld.ProgType,
	ld.FworkCode,
    ld.PwayCode,
    ld.StdCode,
    pe.TNP1,
    pe.TNP2,
    pe.TNP3,
    pe.TNP4
FROM ${ILR_Deds.FQ}.Rulebase.AEC_ApprenticeshipPriceEpisode pe
INNER JOIN ${ILR_Deds.FQ}.Valid.Learner l
    ON pe.Ukprn = l.Ukprn
    AND pe.LearnRefNumber = l.LearnRefNumber
INNER JOIN ${ILR_Deds.FQ}.Valid.LearningDelivery ld
    ON pe.Ukprn = ld.Ukprn
    AND pe.LearnRefNumber = ld.LearnRefNumber
    AND pe.PriceEpisodeAimSeqNumber = ld.AimSeqNumber
INNER JOIN @ProvidersToProcess p
	ON pe.UKPRN = p.UKPRN

---------------------------------------------------------------
-- Commitments
---------------------------------------------------------------
INSERT INTO [Reference].[Commitments]
(CommitmentId, CommitmentVersion, StartDate, StandardCode, ProgrammeType, FrameworkCode, PathwayCode, NegotiatedPrice, EffectiveDate, EmployerAccountId)
SELECT
	CommitmentId, 
    VersionId,
    StartDate,
    StandardCode,
    ProgrammeType,
    FrameworkCode,
    PathwayCode,
    AgreedCost,
    EffectiveFromDate,
    AccountId
FROM ${DAS_Commitments.FQ}.dbo.DasCommitments
WHERE CommitmentId IN (
    SELECT DISTINCT CommitmentId 
    FROM ${DataLock_Deds.FQ}.DataLock.PriceEpisodeMatch pem
    INNER JOIN @ProvidersToProcess p
        ON pem.UKPRN = p.UKPRN
	INNER JOIN @PeriodToProcess pp
		ON pem.CollectionPeriodName = pp.CollectionPeriodName
		AND pem.CollectionPeriodMonth = pp.CollectionPeriodMonth
		AND pem.CollectionPeriodYear = pp.CollectionPeriodYear
)
