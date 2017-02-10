DECLARE @ProvidersToProcess TABLE (UKPRN bigint)
INSERT INTO @ProvidersToProcess
(UKPRN)
SELECT
	p.UKPRN
FROM Reference.Providers

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

---------------------------------------------------------------
-- ValidationError
---------------------------------------------------------------
INSERT INTO [Reference].[ValidationError]
(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear)
SELECT
	Ukprn, 
	LearnRefNumber, 
	AimSeqNumber, 
	RuleId, 
	PriceEpisodeIdentifier, 
	CollectionPeriodName, 
	CollectionPeriodMonth, 
	CollectionPeriodYear
FROM ${DataLock_Deds.FQ}.DataLock.ValidationError ve
INNER JOIN @ProvidersToProcess p
	ON ve.UKPRN = p.UKPRN