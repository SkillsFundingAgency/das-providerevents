CREATE TABLE [dbo].[DataLockEventApprenticeship]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DataLockEventId] INT NOT NULL, 
    [Version] VARCHAR(50) NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [StandardCode] BIGINT NULL, 
    [ProgrammeType] INT NULL, 
    [FrameworkCode] INT NULL, 
    [PathwayCode] INT NULL, 
    [NegotiatedPrice] DECIMAL(12, 5) NOT NULL, 
    [EffectiveDate] DATETIME NOT NULL
)
