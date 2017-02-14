using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventRepository
    {
        DataLockEventEntity[] GetLastSeenEvents();

        long WriteDataLockEvent(DataLockEventEntity @event);
    }
}