using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventErrorRepository
    {
        DataLockEventErrorEntity[] GetDatalockEventErrors(long eventId);
    }
}