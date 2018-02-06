CREATE TABLE [dbo].[LastDataLock]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Ukprn] BIGINT NOT NULL, 
    [LearnerReferenceNumber] VARCHAR(100) NOT NULL, 
    [AimSequenceNumber] BIGINT NOT NULL, 
    [PriceEpisodeIdentifier] VARCHAR(25) NOT NULL, 
    [ErrorCodes] VARCHAR(MAX) NULL, 
    [Commitments] VARCHAR(MAX) NULL
)
