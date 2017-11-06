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

