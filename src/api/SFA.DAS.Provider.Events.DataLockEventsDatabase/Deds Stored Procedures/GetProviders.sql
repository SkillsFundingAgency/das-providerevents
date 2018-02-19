IF EXISTS ( SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[DataLock].[GetProviders]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [DataLock].[GetProviders];
GO --split here

CREATE PROCEDURE [DataLock].[GetProviders]
AS
BEGIN
	SELECT
        [p].[UKPRN] AS [Ukprn],
		[fd].[SubmittedTime] AS [IlrSubmissionDateTime],
		[fd].[Filename] AS [IlrFileName]
	FROM [Valid].[LearningProvider] p
		JOIN [dbo].[FileDetails] fd
			ON p.UKPRN = fd.UKPRN
		JOIN (
			SELECT MAX(ID) AS ID FROM [dbo].[FileDetails] GROUP BY UKPRN
		) LatestByUkprn
			ON fd.ID = LatestByUkprn.ID
END