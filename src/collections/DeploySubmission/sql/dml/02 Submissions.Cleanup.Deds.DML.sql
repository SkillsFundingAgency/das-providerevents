DELETE FROM ${DAS_ProviderEvents.FQ}.[Submissions].[LastSeenVersion]
    WHERE [Ukprn] IN (SELECT DISTINCT lp.[Ukprn] FROM [Valid].[LearningProvider] lp)
GO

