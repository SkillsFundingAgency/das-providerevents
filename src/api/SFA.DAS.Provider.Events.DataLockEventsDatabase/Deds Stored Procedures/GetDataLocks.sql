CREATE PROCEDURE [DataLock].[GetDataLocks] @ukprn BIGINT,
    @page INT,
    @pageSize INT
AS
BEGIN
    SELECT pem.UKPRN,
        pem.LearnRefNumber,
        pem.PriceEpisodeIdentifier,
        (
            SELECT '[' + STRING_AGG('"' + err.RuleId + '"', ',') + ']'
            FROM DataLock.ValidationError err
            WHERE err.Ukprn = pem.Ukprn
                AND err.LearnRefNumber = pem.LearnRefNumber
                AND err.PriceEpisodeIdentifier = pem.PriceEpisodeIdentifier
            ),
        '[' + STRING_AGG(cast(pem.CommitmentId AS SYSNAME), ',') + ']'
    FROM DataLock.PriceEpisodeMatch pem
    WHERE pem.Ukprn = @ukprn
    GROUP BY pem.UKPRN,
        pem.LearnRefNumber,
        pem.PriceEpisodeIdentifier,
        pem.AimSeqNumber
    ORDER BY pem.LearnRefNumber,
        pem.PriceEpisodeIdentifier,
        pem.AimSeqNumber 
	OFFSET(@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
END