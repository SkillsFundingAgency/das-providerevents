using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventPeriodRepository
    {
        DataLockEventPeriodEntity[] GetDataLockEventPeriods(long eventId);

        void WriteDataLockEventPeriod(DataLockEventPeriodEntity period);
    }
}