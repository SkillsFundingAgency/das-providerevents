using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsCommitmentRepository : DcfsRepository, ICommitmentRepository
    {
        private const string Source = "Reference.DasCommitments";
        private const string Columns = "CommitmentId," +
                                       "VersionId AS CommitmentVersion," +
                                       "AccountId AS EmployerAccountId," +
                                       "StartDate," +
                                       "StandardCode," +
                                       "ProgrammeType," +
                                       "FrameworkCode," +
                                       "PathwayCode," +
                                       "AgreedCost AS NegotiatedPrice," +
                                       "EffectiveFrom AS EffectiveDate";
        private const string SelectCommitmentVersions = "SELECT " + Columns + " FROM " + Source +
            " WHERE CommitmentId = @commitmentId";

        public DcfsCommitmentRepository(string connectionString)
            : base(connectionString)
        {
        }

        public CommitmentEntity[] GetCommitmentVersions(long commitmentId)
        {
            return Query<CommitmentEntity>(SelectCommitmentVersions, new { commitmentId });
        }
    }
}