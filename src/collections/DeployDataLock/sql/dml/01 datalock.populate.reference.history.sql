DECLARE @ProvidersToProcess TABLE (UKPRN bigint)
INSERT INTO @ProvidersToProcess
(UKPRN)
SELECT
	p.UKPRN
FROM DataLock.vw_Providers p

DECLARE @LastestDataLockEvents TABLE (EventId bigint)
INSERT INTO @LastestDataLockEvents
SELECT
	MAX(Id)
FROM ${DAS_ProviderEvents.FQ}.DataLock.DataLockEvents
GROUP BY UKPRN, LearnRefNumber, PriceEpisodeIdentifier

---------------------------------------------------------------
-- DataLockEvents
---------------------------------------------------------------
INSERT INTO Reference.DataLockEvents
(Id,DataLockEventId, ProcessDateTime, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, 
PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, 
IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice, IlrPriceEffectiveDate)
SELECT
	dle.Id,
	dle.DataLockEventId,
	dle.ProcessDateTime, 
	dle.IlrFileName, 
    dle.SubmittedDateTime, 
    dle.AcademicYear, 
	dle.UKPRN, 
	dle.ULN, 
	dle.LearnRefNumber, 
	dle.AimSeqNumber, 
	dle.PriceEpisodeIdentifier, 
	dle.CommitmentId, 
	dle.EmployerAccountId, 
	dle.EventSource, 
	dle.HasErrors, 
	dle.IlrStartDate, 
	dle.IlrStandardCode, 
	dle.IlrProgrammeType, 
	dle.IlrFrameworkCode, 
	dle.IlrPathwayCode, 
	dle.IlrTrainingPrice, 
	dle.IlrEndpointAssessorPrice,
	dle.IlrPriceEffectiveDate
	
FROM ${DAS_ProviderEvents.FQ}.DataLock.DataLockEvents dle
INNER JOIN @LastestDataLockEvents le
	ON dle.Id = le.EventId


---------------------------------------------------------------
-- DataLockEventPeriods
---------------------------------------------------------------
INSERT INTO Reference.DataLockEventPeriods
(DataLockEventId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, CommitmentVersion, IsPayable, TransactionType)
SELECT
	dlep.DataLockEventId, 
	dlep.CollectionPeriodName, 
	dlep.CollectionPeriodMonth, 
	dlep.CollectionPeriodYear, 
	dlep.CommitmentVersion, 
	dlep.IsPayable,
    dlep.TransactionType
FROM ${DAS_ProviderEvents.FQ}.DataLock.DataLockEventPeriods dlep
INNER JOIN Reference.DataLockEvents dle 
On dle.DataLockEventId = dlep.DataLockEventId 

---------------------------------------------------------------
-- DataLockEventErrors
---------------------------------------------------------------
INSERT INTO Reference.DataLockEventErrors
(DataLockEventId, ErrorCode, SystemDescription)
SELECT
	dlee.DataLockEventId, 
	dlee.ErrorCode, 
	dlee.SystemDescription
FROM ${DAS_ProviderEvents.FQ}.DataLock.DataLockEventErrors dlee
INNER JOIN Reference.DataLockEvents dle 
On dle.DataLockEventId = dlee.DataLockEventId 


---------------------------------------------------------------
-- DataLockEventCommitmentVersions
---------------------------------------------------------------
INSERT INTO Reference.DataLockEventCommitmentVersions
(DataLockEventId, CommitmentVersion, CommitmentStartDate, CommitmentStandardCode, CommitmentProgrammeType, 
CommitmentFrameworkCode, CommitmentPathwayCode, CommitmentNegotiatedPrice, CommitmentEffectiveDate)
SELECT
	dlecv.DataLockEventId, 
	dlecv.CommitmentVersion, 
	dlecv.CommitmentStartDate, 
	dlecv.CommitmentStandardCode, 
	dlecv.CommitmentProgrammeType, 
	dlecv.CommitmentFrameworkCode, 
	dlecv.CommitmentPathwayCode, 
	dlecv.CommitmentNegotiatedPrice, 
	dlecv.CommitmentEffectiveDate
FROM ${DAS_ProviderEvents.FQ}.DataLock.DataLockEventCommitmentVersions dlecv
INNER JOIN Reference.DataLockEvents dle 
On dle.DataLockEventId = dlecv.DataLockEventId 

