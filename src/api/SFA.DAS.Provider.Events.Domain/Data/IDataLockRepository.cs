using System;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Domain.Data
{
    public interface IDataLockRepository
    {
        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsSinceId(int eventId, int page, int pageSize);
        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsSinceTime(DateTime time, int page, int pageSize);

        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountSinceId(string employerAccountId, int eventId, int page, int pageSize);
        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountSinceTime(string employerAccountId, DateTime time, int page, int pageSize);

        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForProviderSinceId(long ukprn, int eventId, int page, int pageSize);
        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForProviderSinceTime(long ukprn, DateTime time, int page, int pageSize);

        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceId(string employerAccountId, long ukprn, int eventId, int page, int pageSize);
        Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceTime(string employerAccountId, long ukprn, DateTime time, int page, int pageSize);

        Task<DataLockEventErrorEntity[]> GetDataLockErrorsForEvent(long eventId);
        Task<DataLockEventPeriodEntity[]> GetDataLockPeriodsForEvent(long eventId);
        Task<DataLockEventApprenticeshipEntity[]> GetDataLockApprenticeshipsForEvent(long eventId);
    }
}