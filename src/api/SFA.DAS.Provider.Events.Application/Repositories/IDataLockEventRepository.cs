using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface IDataLockEventRepository
    {
        // from web api
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEvents(long? sinceEventId, DateTime? sinceTime, string employerAccountId, long? ukprn , int page, int pageSize);


        // from worker
        Task<IList<ProviderEntity>> GetProviders();

        Task<PageOfResults<DataLockEntity>> GetLastDataLocks(long ukprn, int page, int pageSize);

        Task WriteDataLocks(IList<DataLockEntity> dataLocks);
        Task UpdateDataLocks(IList<DataLockEntity> dataLocks);

        Task WriteDataLockEvents(IList<DataLockEventEntity> events);
        
    }
}
