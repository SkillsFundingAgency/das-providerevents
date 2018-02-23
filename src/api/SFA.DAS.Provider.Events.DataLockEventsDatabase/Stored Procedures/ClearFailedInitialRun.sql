CREATE PROCEDURE [DataLockEvents].[ClearFailedInitialRun] @ukprn BIGINT
AS
BEGIN
    IF NOT EXISTS (
            SELECT 1
            FROM [DataLockEvents].[ProcessorRun] r
            WHERE r.Ukprn = @ukprn
                AND isnull(IsSuccess, 1) = 1
            )
        AND EXISTS (
            SELECT 1
            FROM [DataLockEvents].[Provider] r
            WHERE r.Ukprn = @ukprn
                AND isnull(RequiresInitialImport, 0) = 1
            )
    BEGIN
        DELETE
        FROM [DataLockEvents].[DataLockEvent]
        WHERE [Ukprn] = @ukprn

        DELETE
        FROM [DataLockEvents].[LastDataLock]
        WHERE [Ukprn] = @ukprn
    END
END
