CREATE PROCEDURE [DataLockEvents].[UpdateProvider]
	@Ukprn bigint,
	@IlrSubmissionDateTime datetime
AS
BEGIN
	IF EXISTS(SELECT 1 FROM [DataLockEvents].[Provider] WHERE Ukprn = @Ukprn)
		UPDATE [DataLockEvents].[Provider] 
		SET IlrSubmissionDateTime = @IlrSubmissionDateTime
		WHERE Ukprn = @Ukprn
	ELSE
		INSERT INTO [DataLockEvents].[Provider] (Ukprn, IlrSubmissionDateTime)
		VALUES (@Ukprn, @IlrSubmissionDateTime)
END