using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface IDataLockRepository
    {
        Task<IList<ProviderEntity>> GetProviders();

        Task<PageOfResults<DataLockValidationErrorEntity>> GetDataLockValidationErrors(long ukprn, int pageNumber, int pageSize);

        Task<PageOfResults<DataLockPriceEpisodeMatchEntity>> GetDataLockPriceEpisodeMatch(long ukprn, int pageNumber, int pageSize);
    }
}