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