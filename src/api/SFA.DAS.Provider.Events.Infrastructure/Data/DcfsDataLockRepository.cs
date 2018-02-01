using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsDataLockRepository : DcfsRepository, IDataLockRepository
    {
        private const string GetProvidersSql = @"    
            SELECT
                [p].[UKPRN] AS [Ukprn],
		        [fd].[SubmittedTime] AS [IlrSubmissionDateTime]
	        FROM [Valid].[LearningProvider] p
		        JOIN [dbo].[FileDetails] fd
			        ON p.UKPRN = fd.UKPRN
		        JOIN (
			        SELECT MAX(ID) AS ID FROM [dbo].[FileDetails] GROUP BY UKPRN
		        ) LatestByUkprn
			        ON fd.ID = LatestByUkprn.ID ";

        public async Task<IList<ProviderEntity>> GetProviders()
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                return (await connection.QueryAsync<ProviderEntity>(GetProvidersSql).ConfigureAwait(false)).ToList();
            }
        }

        public Task<PageOfResults<DataLockValidationErrorEntity>> GetDataLockValidationErrors(long ukprn, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PageOfResults<DataLockPriceEpisodeMatchEntity>> GetDataLockPriceEpisodeMatch(long ukprn, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}