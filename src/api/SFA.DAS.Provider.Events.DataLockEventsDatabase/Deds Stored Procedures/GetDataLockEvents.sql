IF EXISTS ( SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[DataLock].[GetDataLockEvents]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [DataLock].[GetDataLockEvents];
GO --split here

CREATE PROCEDURE [DataLock].[GetDataLockEvents]
	@ukprn bigint,
	@offset int,
	@pageSize int,
	@totalPages int out
AS
BEGIN
	SELECT
		@totalPages = ceiling(count(ev.Id) / cast(@pageSize as float))
	FROM
		[DataLock].[DataLockEvents] ev
    WHERE 
		ev.Ukprn = @ukprn

	SELECT [Id]
		  ,[DataLockEventId]
		  ,[ProcessDateTime]
		  ,[IlrFileName]
		  ,[SubmittedDateTime]
		  ,[AcademicYear]
		  ,[UKPRN] AS [Ukprn]
		  ,[ULN] AS [Uln]
		  ,[LearnRefNumber]
		  ,[AimSeqNumber]
		  ,[PriceEpisodeIdentifier]
		  ,[CommitmentId]
		  ,[EmployerAccountId]
		  ,[EventSource]
		  ,[HasErrors]
		  ,[IlrStartDate]
		  ,[IlrStandardCode]
		  ,[IlrProgrammeType]
		  ,[IlrFrameworkCode]
		  ,[IlrPathwayCode]
		  ,[IlrTrainingPrice]
		  ,[IlrEndpointAssessorPrice]
		  ,[IlrPriceEffectiveFromDate]
		  ,[IlrPriceEffectiveToDate]
		  ,[Status]
		  ,(
				SELECT '[' + STRING_AGG('"' + [ErrorCode] + '"', ',') + ']' FROM (
					SELECT DISTINCT err.[ErrorCode]
					FROM [DataLock].[DataLockEventErrors] err
					WHERE err.[DataLockEventId] = ev.[DataLockEventId]
				) AS SQ
			) AS [ErrorCodes]
	  FROM [DataLock].[DataLockEvents] ev
      WHERE ev.Ukprn = @ukprn
	  ORDER BY ev.Id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
END
