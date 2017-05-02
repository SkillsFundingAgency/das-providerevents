using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventRepository
    {
        DataLockEventEntity[] GetProviderLastSeenEvents(long ukprn);


        void BulkWriteDataLockEvents(DataLockEventEntity[] events);
    }
}