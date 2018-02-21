CREATE TABLE [DataLockEvents].[ProcessorRun]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Ukprn] BIGINT NOT NULL, 
    [IlrSubmissionDateTime] DATETIME NOT NULL, 
    [StartTimeUtc] DATETIME NOT NULL, 
    [FinishTimeUtc] DATETIME NULL, 
    [IsInitialRun] BIT NOT NULL, 
    [IsSuccess] BIT NULL, 
    [Error] NVARCHAR(MAX) NULL
)
