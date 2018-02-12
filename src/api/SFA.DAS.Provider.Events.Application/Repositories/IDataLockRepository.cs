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

        Task<IList<DataLockEntity>> GetDataLocks(long ukprn, int page, int pageSize);
        //Task<IList<DataLockValidationErrorEntity>> GetDataLockValidationErrors(IList<Api.Types.DataLock> dataLocks);
        //Task<IList<DataLockPriceEpisodeMatchEntity>> GetDataLockPriceEpisodeMatch(IList<Api.Types.DataLock> dataLocks);

        //Task<PageOfResults<DataLockPriceEpisodeMatchEntity>> GetDataLockPriceEpisodeMatch(long ukprn, int pageNumber, int pageSize);
    }
}