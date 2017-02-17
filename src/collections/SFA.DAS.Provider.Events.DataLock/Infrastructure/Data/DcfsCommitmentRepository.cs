using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsCommitmentRepository : DcfsRepository, ICommitmentRepository
    {
        private const string Source = "Reference.Commitments";
        private const string Columns = "CommitmentId," +
                                       "CommitmentVersion," +
                                       "EmployerAccountId," +
                                       "StartDate," +
                                       "StandardCode," +
                                       "ProgrammeType," +
                                       "FrameworkCode," +
                                       "PathwayCode," +
                                       "NegotiatedPrice," +
                                       "EffectiveDate";
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