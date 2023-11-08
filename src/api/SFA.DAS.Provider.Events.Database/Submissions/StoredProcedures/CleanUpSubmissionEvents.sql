CREATE PROCEDURE [Submissions].[CleanUpSubmissionEvents] (@ukprn int) AS
BEGIN 
	
DELETE FROM [Submissions].[LastSeenVersion]
    WHERE UKPRN = @ukprn
		
END