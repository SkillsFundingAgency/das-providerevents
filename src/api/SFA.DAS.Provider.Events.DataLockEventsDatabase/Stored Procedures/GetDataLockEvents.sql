CREATE PROCEDURE [DataLockEvents].[GetDataLockEvents]
	@sinceEventId bigint,
	@sinceTime datetime,
	@employerAccountId bigint,
	@ukprn bigint,
	@offset int,
	@pageSize int,
	@totalPages int out
AS
BEGIN
	SELECT
		@totalPages = ceiling(count(ev.Id) / cast(@pageSize as float))
	FROM
		[DataLockEvents].[DataLockEvent] ev
	WHERE
		(@sinceEventId is null or ev.Id > @sinceEventId)
		and (@sinceTime is null or ev.ProcessDateTime > @sinceTime)
		and (@employerAccountId is null or ev.EmployerAccountId = @employerAccountId)
		and (@ukprn is null or ev.Ukprn = @ukprn)

	SELECT
		[Id],
		[ProcessDateTime],
		[Status],
		[IlrFileName],
		[Ukprn],
		[Uln],
		[LearnRefNumber],
		[AimSeqNumber],
		[PriceEpisodeIdentifier],
		[CommitmentId],
		[EmployerAccountId],
		[HasErrors],
		[IlrStartDate],
		[IlrStandardCode],
		[IlrProgrammeType],
		[IlrFrameworkCode],
		[IlrPathwayCode],
		[IlrTrainingPrice],
		[IlrEndpointAssessorPrice],
		[IlrPriceEffectiveFromDate],
		[IlrPriceEffectiveToDate],
		[ErrorCodes]
	FROM
		[DataLockEvents].[DataLockEvent] ev
	WHERE
		(@sinceEventId is null or ev.Id > @sinceEventId)
		and (@sinceTime is null or ev.ProcessDateTime > @sinceTime)
		and (@employerAccountId is null or ev.EmployerAccountId = @employerAccountId)
		and (@ukprn is null or ev.Ukprn = @ukprn)
	ORDER BY ev.Id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
END

