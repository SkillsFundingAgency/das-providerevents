CREATE TABLE [DataLock].[DataLockEventPeriods](
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[CommitmentVersion] [varchar](25) NOT NULL,
	[IsPayable] [bit] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[TransactionTypesFlag] [int] NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DataLockEventPeriods_DataLockId] ON [DataLock].[DataLockEventPeriods]
(
	[DataLockEventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO