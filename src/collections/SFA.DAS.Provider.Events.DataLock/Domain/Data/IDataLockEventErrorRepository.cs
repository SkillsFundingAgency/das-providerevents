using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventErrorRepository
    {
        DataLockEventErrorEntity[] GetDatalockEventErrors(Guid eventId);

        void BulkWriteDataLockEventError(DataLockEventErrorEntity[] errors);
    }
}