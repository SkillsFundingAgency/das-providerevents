using System;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface IDataLockRepository
    {
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
    }
}