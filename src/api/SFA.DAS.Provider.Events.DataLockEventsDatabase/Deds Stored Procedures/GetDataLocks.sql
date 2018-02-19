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
	SELECT pem.[ErrorCodes],
		pem.[AimSequenceNumber],
		pem.[CommitmentId] AS [ApprenticeshipId],
		pem.[IsSuccess],
		pem.[LearnerReferenceNumber],
		c.[EmployerAccountId],
		dpe.*
	FROM (
		SELECT pem.UKPRN [Ukprn],
			pem.LearnRefNumber [LearnerReferenceNumber],
			pem.PriceEpisodeIdentifier [PriceEpisodeIdentifier],
			pem.CommitmentId,
			(
				SELECT '[' + STRING_AGG('"' + RuleId + '"', ',') + ']' FROM (
					SELECT DISTINCT err.RuleId
					FROM DataLock.ValidationError err
					WHERE err.Ukprn = pem.Ukprn
						AND err.LearnRefNumber = pem.LearnRefNumber
						AND err.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
				) AS SQ
			) AS [ErrorCodes],
			pem.IsSuccess,
			pem.AimSeqNumber [AimSequenceNumber]
		FROM DataLock.PriceEpisodeMatch pem
		WHERE pem.Ukprn = @ukprn
		GROUP BY pem.UKPRN,
			pem.LearnRefNumber,
			pem.PriceEpisodeIdentifier,
			pem.AimSeqNumber,
			pem.CommitmentId,
			pem.IsSuccess
		) AS pem
	INNER JOIN (
		SELECT 
			l.[Ukprn],
			l.[LearnRefNumber] AS [LearnRefNumber],
			ape.[PriceEpisodeIdentifier],
			l.[ULN] AS [Uln],
			ld.LearnStartDate AS IlrStartDate,
			ld.[StdCode] AS IlrStandardCode,
			ld.[ProgType] AS IlrProgrammeType,
			ld.[FworkCode] AS IlrFrameworkCode,
			ld.[PwayCode] AS IlrPathwayCode,
			CASE WHEN ISNULL(ape.Tnp1, 0) > 0 THEN ape.Tnp1 ELSE ape.Tnp3 END AS IlrTrainingPrice,
			CASE WHEN ISNULL(ape.Tnp1, 0) > 0 THEN ape.Tnp2 ELSE ape.Tnp4 END AS IlrEndpointAssessorPrice,
			ape.[TNP1],
			ape.[TNP2],
			ape.[TNP3],
			ape.[TNP4],
			ape.EpisodeEffectiveTNPStartDate AS IlrPriceEffectiveFromDate,
			et.EffectiveTo AS IlrPriceEffectiveToDate
		FROM 
			[Rulebase].[AEC_ApprenticeshipPriceEpisode] ape
			INNER JOIN [Valid].[Learner] l ON ape.[Ukprn] = l.[Ukprn]
				AND ape.[LearnRefNumber] = l.[LearnRefNumber]
			INNER JOIN [Valid].[LearningDelivery] ld ON ape.[Ukprn] = ld.[Ukprn]
				AND ape.[LearnRefNumber] = ld.[LearnRefNumber]
				AND ape.[PriceEpisodeAimSeqNumber] = ld.[AimSeqNumber]
			LEFT JOIN (
				SELECT x.Ukprn,
					x.PriceEpisodeIdentifier,
					x.LearnRefNumber,
					x.PriceEpisodeAimSeqNumber,
					DATEADD(DD, - 1, MIN(y.EpisodeEffectiveTNPStartDate)) EffectiveTo
				FROM [Rulebase].[AEC_ApprenticeshipPriceEpisode] x
				LEFT JOIN [Rulebase].[AEC_ApprenticeshipPriceEpisode] y ON x.LearnRefNumber = y.LearnRefNumber
					AND y.EpisodeEffectiveTNPStartDate > x.EpisodeEffectiveTNPStartDate
				GROUP BY x.ukprn,
					x.PriceEpisodeIdentifier,
					x.LearnRefNumber,
					x.PriceEpisodeAimSeqNumber,
					x.EpisodeEffectiveTNPStartDate
				) et ON ape.Ukprn = et.Ukprn
				AND ape.PriceEpisodeIdentifier = et.PriceEpisodeIdentifier
				AND ape.LearnRefNumber = et.LearnRefNumber
				AND ape.PriceEpisodeAimSeqNumber = et.PriceEpisodeAimSeqNumber
		WHERE 
			ape.PriceEpisodeContractType = 'Levy Contract'
			AND l.Ukprn = @ukprn
		) AS [dpe] ON dpe.Ukprn = pem.Ukprn
		AND dpe.LearnRefNumber = pem.LearnerReferenceNumber
		AND dpe.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
	INNER JOIN (
		select 
			c.CommitmentId, 
			max(c.AccountId) as EmployerAccountId 
		from 
			dbo.DasCommitments c 
		group by 
			c.CommitmentId
		) c ON pem.CommitmentId = c.CommitmentId
	ORDER BY pem.LearnerReferenceNumber,
		pem.PriceEpisodeIdentifier,
		pem.AimSequenceNumber
	OFFSET(@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
END