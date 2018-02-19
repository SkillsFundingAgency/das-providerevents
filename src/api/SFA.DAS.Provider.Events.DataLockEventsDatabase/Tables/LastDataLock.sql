CREATE TABLE [DataLockEvents].[LastDataLock]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Ukprn] BIGINT NOT NULL, 
    [LearnerReferenceNumber] VARCHAR(100) NOT NULL, 
    [AimSequenceNumber] BIGINT NOT NULL, 
    [PriceEpisodeIdentifier] VARCHAR(25) NOT NULL, 
    [ErrorCodes] VARCHAR(MAX) NULL, 
    [DeletedUtc] DATETIME NULL,
	[Uln] BIGINT NOT NULL,
    [ApprenticeshipId] BIGINT NOT NULL,
    [EmployerAccountId] BIGINT NOT NULL,
    [IlrStartDate] DATETIME,
    [IlrStandardCode] BIGINT,
    [IlrProgrammeType] INT,
    [IlrFrameworkCode] INT,
    [IlrPathwayCode] INT,
    [IlrTrainingPrice] DECIMAL(15,5),
    [IlrEndpointAssessorPrice]  DECIMAL(15,5),
    [IlrPriceEffectiveFromDate] DATETIME,
    [IlrPriceEffectiveToDate] DATETIME
)
