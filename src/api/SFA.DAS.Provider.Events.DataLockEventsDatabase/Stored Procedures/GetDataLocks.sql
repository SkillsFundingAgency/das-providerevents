CREATE PROCEDURE [DataLockEvents].[GetDataLocks] 
	@ukprn BIGINT,
    @page INT,
    @pageSize INT,
	@totalPages int out
AS
BEGIN
    SELECT @totalPages = ceiling(count(dl.Id) / cast(@pageSize as float))
    FROM [DataLockEvents].[LastDataLock] dl
    WHERE dl.Ukprn = @ukprn
		AND dl.DeletedUtc is null

    SELECT *
    FROM [DataLockEvents].[LastDataLock] dl
    WHERE dl.Ukprn = @ukprn
		AND dl.DeletedUtc is null
    ORDER BY dl.Id
	OFFSET(@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
END