using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventRepository : DcfsRepository, IDataLockEventRepository
    {
        private const string Source = "Reference.DataLockEvents";
        private const string Columns = "Id," +
                                       "ProcessDateTime," +
                                       "IlrFileName," +
                                       "SubmittedDateTime," +
                                       "AcademicYear," +
                                       "UKPRN," +
                                       "ULN," +
                                       "LearnRefNumber," +
                                       "AimSeqNumber," +
                                       "PriceEpisodeIdentifier," +
                                       "CommitmentId," +
                                       "EmployerAccountId," +
                                       "EventSource," +
                                       "HasErrors," +
                                       "IlrStartDate," +
                                       "IlrStandardCode," +
                                       "IlrProgrammeType," +
                                       "IlrFrameworkCode," +
                                       "IlrPathwayCode," +
                                       "IlrTrainingPrice," +
                                       "IlrEndpointAssessorPrice";
        private const string SelectLastSeenEvents = "SELECT " + Columns + " FROM " + Source;

        public DcfsDataLockEventRepository(string connectionString)
            : base(connectionString)
        {
        }

        public DataLockEventEntity[] GetLastSeenEvents()
        {
            return Query<DataLockEventEntity>(SelectLastSeenEvents);
        }
    }
}