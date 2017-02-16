DECLARE @ProvidersToProcess TABLE (UKPRN bigint)
INSERT INTO @ProvidersToProcess
(UKPRN)
SELECT
	p.UKPRN
FROM Reference.Providers p

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
FROM [DasProviderDataLockEvents_Deds].DataLock.PriceEpisodeMatch pem
INNER JOIN @ProvidersToProcess p
	ON pem.UKPRN = p.UKPRN

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
FROM [DasProviderDataLockEvents_Deds].DataLock.PriceEpisodePeriodMatch pepm
INNER JOIN @ProvidersToProcess p
	ON pepm.UKPRN = p.UKPRN

---------------------------------------------------------------
-- ValidationError
---------------------------------------------------------------
INSERT INTO [Reference].[ValidationError]
(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear)
SELECT
	ve.Ukprn, 
	LearnRefNumber, 
	AimSeqNumber, 
	RuleId, 
	PriceEpisodeIdentifier, 
	CollectionPeriodName, 
	CollectionPeriodMonth, 
	CollectionPeriodYear
FROM [DasProviderDataLockEvents_Deds].DataLock.ValidationError ve
INNER JOIN @ProvidersToProcess p
	ON ve.UKPRN = p.UKPRN

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
FROM [DasProviderDataLockEvents_Deds].Rulebase.AEC_ApprenticeshipPriceEpisode pe
INNER JOIN [DasProviderDataLockEvents_Deds].Valid.Learner l
    ON pe.Ukprn = l.Ukprn
    AND pe.LearnRefNumber = l.LearnRefNumber
INNER JOIN [DasProviderDataLockEvents_Deds].Valid.LearningDelivery ld
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
FROM [DasProviderDataLockEvents_Deds].dbo.DasCommitments
WHERE CommitmentId IN (
    SELECT DISTINCT CommitmentId 
    FROM [DasProviderDataLockEvents_Deds].DataLock.PriceEpisodeMatch pem
    INNER JOIN @ProvidersToProcess p
        ON pem.UKPRN = p.UKPRN
)
