using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IDataLockEventCommitmentVersionRepository
    {
        DataLockEventCommitmentVersionEntity[] GetDataLockEventCommitmentVersions(long eventId);

        void WriteDataLockEventCommitmentVersion(DataLockEventCommitmentVersionEntity version);
    }
}