using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventPeriodRepository
    {
        DataLockEventPeriodEntity[] GetDataLockEventPeriods(Guid eventId);

        void WriteDataLockEventPeriod(DataLockEventPeriodEntity period);

        void BulkWriteDataLockEventPeriods(DataLockEventPeriodEntity[] periods);
    }
}