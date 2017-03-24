using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventCommitmentVersionRepository : DcfsRepository, IDataLockEventCommitmentVersionRepository
    {
        private const string Source = "Reference.DataLockEventCommitmentVersions";
        private const string Columns = "DataLockEventId," +
                                       "CommitmentVersion," +
                                       "CommitmentStartDate," +
                                       "CommitmentStandardCode," +
                                       "CommitmentProgrammeType," +
                                       "CommitmentFrameworkCode," +
                                       "CommitmentPathwayCode," +
                                       "CommitmentNegotiatedPrice," +
                                       "CommitmentEffectiveDate";
        private const string SelectEventCommitmentVersions = "SELECT " + Columns + " FROM " + Source + " WHERE DataLockEventId = @eventId";

        public DcfsDataLockEventCommitmentVersionRepository(string connectionString)
            : base(connectionString)
        {
        }

        public DataLockEventCommitmentVersionEntity[] GetDataLockEventCommitmentVersions(long eventId)
        {
            return Query<DataLockEventCommitmentVersionEntity>(SelectEventCommitmentVersions, new { eventId });
        }

        public void WriteDataLockEventCommitmentVersion(DataLockEventCommitmentVersionEntity version)
        {
            Execute("INSERT INTO DataLockEvents.DataLockEventCommitmentVersions " +
                    "(DataLockEventId, CommitmentVersion, CommitmentStartDate, CommitmentStandardCode, CommitmentProgrammeType, CommitmentFrameworkCode, " +
                    "CommitmentPathwayCode, CommitmentNegotiatedPrice, CommitmentEffectiveDate) " +
                    "VALUES " +
                    "(@DataLockEventId, @CommitmentVersion, @CommitmentStartDate, @CommitmentStandardCode, @CommitmentProgrammeType, @CommitmentFrameworkCode, " +
                    "@CommitmentPathwayCode, @CommitmentNegotiatedPrice, @CommitmentEffectiveDate) ",
                    version);
        }
    }
}