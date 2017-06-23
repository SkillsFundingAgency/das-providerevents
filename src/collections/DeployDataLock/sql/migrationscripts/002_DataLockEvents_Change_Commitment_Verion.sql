
ALTER TABLE  [DataLock].[DataLockEventPeriods]
ALTER COLUMN CommitmentVersion	varchar(25) NOT NULL
GO


ALTER TABLE  [DataLock].[DataLockEventCommitmentVersions]
ALTER COLUMN CommitmentVersion	varchar(25) NOT NULL
GO

