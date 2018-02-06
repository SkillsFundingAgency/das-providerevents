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
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsSinceId(long eventId, int page, int pageSize);
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsSinceTime(DateTime time, int page, int pageSize);

        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForAccountSinceId(string employerAccountId, long eventId, int page, int pageSize);
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForAccountSinceTime(string employerAccountId, DateTime time, int page, int pageSize);

        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForProviderSinceId(long ukprn, long eventId, int page, int pageSize);
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForProviderSinceTime(long ukprn, DateTime time, int page, int pageSize);

        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceId(string employerAccountId, long ukprn, long eventId, int page, int pageSize);
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceTime(string employerAccountId, long ukprn, DateTime time, int page, int pageSize);

        Task<DataLockEventErrorEntity[]> GetDataLockErrorsForEvents(string[] eventIds);
        Task<DataLockEventPeriodEntity[]> GetDataLockPeriodsForEvent(string[] eventIds);
        Task<DataLockEventApprenticeshipEntity[]> GetDataLockApprenticeshipsForEvent(string[] eventIds);


        // from worker
        Task<IList<ProviderEntity>> GetProviders();

        Task<PageOfResults<DataLockEntity>> GetLastDataLocks(long ukprn, int page, int pageSize);

        Task WriteDataLocks(IList<DataLockEntity> dataLocks);

        Task WriteDataLockEvents(IList<DataLockEventEntity> events);
        
    }
}
