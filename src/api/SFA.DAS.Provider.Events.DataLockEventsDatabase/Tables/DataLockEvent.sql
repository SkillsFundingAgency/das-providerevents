﻿CREATE TABLE [dbo].[DataLockEvent]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [ProcessDateTime] DATETIME NOT NULL, 
    [Status] INT NOT NULL, 
    [IlrFileName] NVARCHAR(50) NOT NULL, 
    [Ukprn] BIGINT NOT NULL, 
    [Uln] BIGINT NOT NULL, 
    [LearnRefNumber] VARCHAR(100) NOT NULL, 
    [AimSeqNumber] BIGINT NOT NULL, 
    [PriceEpisodeIdentifier] VARCHAR(25) NOT NULL, 
    [ApprenticeshipId] BIGINT NOT NULL, 
    [EmployerAccountId] BIGINT NOT NULL, 
    [EventSource] INT NOT NULL, 
    [HasErrors] BIT NOT NULL, 
    [IlrStartDate] DATETIME NULL, 
    [IlrStandardCode] BIGINT NULL, 
    [IlrProgrammeType] INT NULL, 
    [IlrFrameworkCode] INT NULL, 
    [IlrPathwayCode] INT NULL, 
    [IlrTrainingPrice] DECIMAL(12, 5) NULL, 
    [IlrEndpointAssessorPrice] DECIMAL(12, 5) NULL, 
    [IlrPriceEffectiveFromDate] DATETIME NULL, 
    [IlrPriceEffectiveToDate] DATETIME NULL, 
    [ErrorCodes] VARCHAR(MAX) NULL
)
