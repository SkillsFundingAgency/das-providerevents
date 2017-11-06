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