using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventCommitmentVersionRepository
    {
        DataLockEventCommitmentVersionEntity[] GetDataLockEventCommitmentVersions(Guid eventId);

        void WriteDataLockEventCommitmentVersion(DataLockEventCommitmentVersionEntity version);
    }
}