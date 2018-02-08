CREATE PROCEDURE [DataLockEvents].[UpdateDataLocks]
	@dataLocks [DataLockEvents].[DataLockEntity] readonly
AS
BEGIN
  UPDATE [DataLockEvents].[LastDataLock] 
	SET ErrorCodes = dl.ErrorCodes,
		Commitments = dl.Commitments,
		DeletedUtc = dl.DeletedUtc  
  FROM [DataLockEvents].[LastDataLock] 
	JOIN @dataLocks dl ON dl.Ukprn = [DataLockEvents].[LastDataLock].Ukprn
    AND dl.LearnerReferenceNumber = [DataLockEvents].[LastDataLock].LearnerReferenceNumber
    AND dl.AimSequenceNumber = [DataLockEvents].[LastDataLock].AimSequenceNumber
END