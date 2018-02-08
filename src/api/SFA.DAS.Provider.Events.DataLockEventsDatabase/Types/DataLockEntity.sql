CREATE TYPE [DataLockEvents].[DataLockEntity] AS TABLE (
    Ukprn BIGINT,
    LearnerReferenceNumber VARCHAR(12),
    PriceEpisodeIdentifier VARCHAR(25),
    AimSequenceNumber BIGINT,
    ErrorCodes NVARCHAR(MAX),
    Commitments NVARCHAR(MAX),
    DeletedUtc DATETIME
);
