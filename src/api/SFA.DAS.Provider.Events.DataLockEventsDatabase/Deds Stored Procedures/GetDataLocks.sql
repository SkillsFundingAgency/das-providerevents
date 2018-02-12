IF EXISTS ( SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[DataLock].[GetDataLocks]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [DataLock].[GetDataLocks];
GO --split here

CREATE PROCEDURE [DataLock].[GetDataLocks] 
	@ukprn BIGINT,
    @page INT,
    @pageSize INT
AS
BEGIN
	SELECT pem.ErrorCodes,
		pem.Commitments,
		dpe.*
	FROM (
		SELECT pem.UKPRN [Ukprn],
			pem.LearnRefNumber [LearnerReferenceNumber],
			pem.PriceEpisodeIdentifier [PriceEpisodeIdentifier],
			pem.CommitmentId,
			(
				SELECT '[' + STRING_AGG('"' + err.RuleId + '"', ',') + ']'
				FROM DataLock.ValidationError err
				WHERE err.Ukprn = pem.Ukprn
					AND err.LearnRefNumber = pem.LearnRefNumber
					AND err.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
			) [ErrorCodes],
			(
				SELECT '[' + STRING_AGG(cast(pepm.VersionId as varchar(20)), ',') + ']'
				FROM DataLock.PriceEpisodePeriodMatch pepm
				WHERE pem.Ukprn = pepm.Ukprn
					AND pem.LearnRefnumber= pepm.LearnRefnumber
					AND pem.AimSeqNumber = pepm.AimSeqNumber
					AND pem.PriceEpisodeIdentifier = pepm.PriceEpisodeIdentifier 
			) [Commitments],
			pem.AimSeqNumber [AimSequenceNumber]
		FROM DataLock.PriceEpisodeMatch pem
		WHERE pem.Ukprn = @ukprn
		GROUP BY pem.UKPRN,
			pem.LearnRefNumber,
			pem.PriceEpisodeIdentifier,
			pem.AimSeqNumber,
			pem.CommitmentId
		) AS pem
	INNER JOIN (
		SELECT dpe.Ukprn,
			dpe.LearnRefNumber,
			dpe.PriceEpisodeIdentifier,
			dpe.Uln,
			dpe.LearningStartDate IlrStartDate,
			dpe.StandardCode IlrStandardCode,
			dpe.ProgrammeType IlrProgrammeType,
			dpe.FrameworkCode IlrFrameworkCode,
			dpe.PathwayCode IlrPathwayCode,
			CASE WHEN ISNULL(dpe.Tnp1, 0) > 0 THEN dpe.Tnp1 ELSE dpe.Tnp3 END IlrTrainingPrice,
			CASE WHEN ISNULL(dpe.Tnp1, 0) > 0 THEN dpe.Tnp2 ELSE dpe.Tnp4 END IlrEndpointAssessorPrice,
			dpe.StartDate IlrPriceEffectiveFromDate,
			dpe.EffectiveToDate IlrPriceEffectiveToDate
		FROM Reference.DataLockPriceEpisode dpe
		   WHERE dpe.Ukprn = @ukprn
		) AS [dpe] ON dpe.Ukprn = pem.Ukprn
		AND dpe.LearnRefNumber = pem.LearnerReferenceNumber
		AND dpe.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
	ORDER BY pem.LearnerReferenceNumber,
		pem.PriceEpisodeIdentifier,
		pem.AimSequenceNumber
	OFFSET(@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
END