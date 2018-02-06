CREATE PROCEDURE [DataLock].[GetProviders]
AS
BEGIN
	SELECT
        [p].[UKPRN] AS [Ukprn],
		[fd].[SubmittedTime] AS [IlrSubmissionDateTime]
	FROM [Valid].[LearningProvider] p
		JOIN [dbo].[FileDetails] fd
			ON p.UKPRN = fd.UKPRN
		JOIN (
			SELECT MAX(ID) AS ID FROM [dbo].[FileDetails] GROUP BY UKPRN
		) LatestByUkprn
			ON fd.ID = LatestByUkprn.ID
END