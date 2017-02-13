INSERT INTO [Reference].[Providers]
(UKPRN, IlrFilename, SubmittedTime)
SELECT
	p.UKPRN,
	fd.Filename,
	fd.SubmittedTime
FROM ${ILR_Deds.FQ}.Valid.LearningProvider p
INNER JOIN ${ILR_Deds.FQ}.dbo.FileDetails fd
	ON p.UKPRN = fd.UKPRN