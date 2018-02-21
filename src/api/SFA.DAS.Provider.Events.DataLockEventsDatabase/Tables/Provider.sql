CREATE TABLE [DataLockEvents].[Provider]
(
	[Ukprn] BIGINT NOT NULL PRIMARY KEY, 
    [IlrSubmissionDateTime] DATETIME NOT NULL,
	[RequiresInitialImport] BIT NOT NULL DEFAULT(0), 
    [HandledBy] INT NULL
)
