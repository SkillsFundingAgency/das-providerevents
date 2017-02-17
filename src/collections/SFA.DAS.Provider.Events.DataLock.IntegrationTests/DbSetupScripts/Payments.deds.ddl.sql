IF NOT EXISTS (SELECT [schema_id] FROM sys.schemas WHERE name='Payments')
	BEGIN
		EXEC('CREATE SCHEMA Payments')
	END
GO

IF EXISTS (SELECT [object_id] FROM sys.tables WHERE [name] = 'Periods' AND [schema_id] = SCHEMA_ID('Payments'))
	BEGIN
		DROP TABLE Payments.Periods
	END
GO

CREATE TABLE [Payments].[Periods](
	[PeriodName] [char](8) NOT NULL PRIMARY KEY,
	[CalendarMonth] [int] NOT NULL,
	[CalendarYear] [int] NOT NULL,
	[AccountDataValidAt] [datetime] NULL,
	[CommitmentDataValidAt] [datetime] NULL,
	[CompletionDateTime] [datetime] NOT NULL
)
GO
