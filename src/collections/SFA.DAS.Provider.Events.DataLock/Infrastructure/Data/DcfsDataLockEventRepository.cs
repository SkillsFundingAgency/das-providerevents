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

        public long WriteDataLockEvent(DataLockEventEntity @event)
        {
            if (@event.Id < 1)
            {
                @event.Id = QuerySingle<int>("SELECT ISNULL(MAX(Id), 0) FROM DataLock.DataLockEvents") + 1;

                if (@event.Id == 1)
                {
                    @event.Id = QuerySingle<int>("SELECT ISNULL(MaxIdInDeds, 0) FROM Reference.IdentifierSeed WHERE IdentifierName = 'DataLockEvents'") + 1;
                }
            }

            Execute("INSERT INTO DataLock.DataLockEvents " +
                    "(Id, ProcessDateTime, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, " +
                    "PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, " +
                    "IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice) " +
                    "VALUES " +
                    "(@Id, @ProcessDateTime, @IlrFileName, @SubmittedDateTime, @AcademicYear, @UKPRN, @ULN, @LearnRefNumber, @AimSeqNumber, " +
                    "@PriceEpisodeIdentifier, @CommitmentId, @EmployerAccountId, @EventSource, @HasErrors, @IlrStartDate, @IlrStandardCode, " +
                    "@IlrProgrammeType, @IlrFrameworkCode, @IlrPathwayCode, @IlrTrainingPrice, @IlrEndpointAssessorPrice)",
                    @event);

            return @event.Id;
        }
    }
}