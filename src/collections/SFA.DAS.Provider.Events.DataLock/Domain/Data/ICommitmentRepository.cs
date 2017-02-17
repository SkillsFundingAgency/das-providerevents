using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface ICommitmentRepository
    {
        CommitmentEntity[] GetCommitmentVersions(long commitmentId);
    }
}