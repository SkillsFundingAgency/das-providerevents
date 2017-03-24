using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventRepository
    {
        DataLockEventEntity[] GetProviderLastSeenEvents(long ukprn);

        long WriteDataLockEvent(DataLockEventEntity @event);
    }
}