IF EXISTS (SELECT 1 FROM sys.procedures WHERE [name]='GetCurrentEventData' AND [schema_id] =SCHEMA_ID('DataLockEvents'))
	BEGIN
		DROP PROCEDURE DataLockEvents.GetCurrentEventData
	END
GO

CREATE PROCEDURE DataLockEvents.GetCurrentEventData
	@UKPRN bigint
AS
SET NOCOUNT ON

	SELECT * FROM DataLockEvents.DataLockEventsData
	WHERE Ukprn = @UKPRN
	ORDER BY LearnRefNumber, AimSeqNumber, RuleId
GO