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
                                       "IlrEndpointAssessorPrice," +
                                       "IlrPriceEffectiveDate";
        private const string SelectLastSeenEvents = "SELECT " + Columns + " FROM " + Source;
        private const string SelectProviderLastSeenEvents = SelectLastSeenEvents + " WHERE Ukprn = @ukprn";


        public DcfsDataLockEventRepository(string connectionString)
            : base(connectionString)
        {
        }

        public DataLockEventEntity[] GetProviderLastSeenEvents(long ukprn)
        {
            return Query<DataLockEventEntity>(SelectProviderLastSeenEvents, new { ukprn });
        }

        public long WriteDataLockEvent(DataLockEventEntity @event)
        {
            if (@event.Id < 1)
            {
                @event.Id = QuerySingle<int>("SELECT ISNULL(MAX(Id), 0) FROM DataLockEvents.DataLockEvents") + 1;

                if (@event.Id == 1)
                {
                    @event.Id = QuerySingle<int>("SELECT ISNULL(MaxIdInDeds, 0) FROM Reference.IdentifierSeed WHERE IdentifierName = 'DataLockEvents'") + 1;
                }
            }

            Execute("INSERT INTO DataLockEvents.DataLockEvents " +
                    "(Id, ProcessDateTime, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, " +
                    "PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, " +
                    "IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice, IlrPriceEffectiveDate) " +
                    "VALUES " +
                    "(@Id, @ProcessDateTime, @IlrFileName, @SubmittedDateTime, @AcademicYear, @UKPRN, @ULN, @LearnRefNumber, @AimSeqNumber, " +
                    "@PriceEpisodeIdentifier, @CommitmentId, @EmployerAccountId, @EventSource, @HasErrors, @IlrStartDate, @IlrStandardCode, " +
                    "@IlrProgrammeType, @IlrFrameworkCode, @IlrPathwayCode, @IlrTrainingPrice, @IlrEndpointAssessorPrice, @IlrPriceEffectiveDate)",
                    @event);

            return @event.Id;
        }
    }
}