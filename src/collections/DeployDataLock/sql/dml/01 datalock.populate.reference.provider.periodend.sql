INSERT INTO [Reference].[Providers]
(UKPRN, IlrFilename, SubmittedTime)
SELECT
	fd.UKPRN,
	fd.Filename,
	fd.SubmittedTime
FROM ${ILR_Deds.FQ}.dbo.FileDetails fd
JOIN (
    SELECT MAX(ID) AS ID FROM ${ILR_Deds.FQ}.dbo.FileDetails WHERE Success = 1 GROUP BY UKPRN 
) LatestByUkprn
    ON fd.ID = LatestByUkprn.ID