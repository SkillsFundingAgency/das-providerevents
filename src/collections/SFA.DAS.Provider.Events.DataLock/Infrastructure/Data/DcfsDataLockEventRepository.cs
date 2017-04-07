using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventRepository : DcfsRepository, IDataLockEventRepository
    {
        private const string Source = "Reference.DataLockEvents";
        private const string Columns = "Id," +
                                       "DataLockEventId," +
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

        public Guid WriteDataLockEvent(DataLockEventEntity @event)
        {
            @event.DataLockEventId = Guid.NewGuid();

            Execute("INSERT INTO DataLockEvents.DataLockEvents " +
                    "(DataLockEventId, ProcessDateTime, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, " +
                    "PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, " +
                    "IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice, IlrPriceEffectiveDate) " +
                    "VALUES " +
                    "(@dataLockEventId,@ProcessDateTime, @IlrFileName, @SubmittedDateTime, @AcademicYear, @UKPRN, @ULN, @LearnRefNumber, @AimSeqNumber, " +
                    "@PriceEpisodeIdentifier, @CommitmentId, @EmployerAccountId, @EventSource, @HasErrors, @IlrStartDate, @IlrStandardCode, " +
                    "@IlrProgrammeType, @IlrFrameworkCode, @IlrPathwayCode, @IlrTrainingPrice, @IlrEndpointAssessorPrice, @IlrPriceEffectiveDate)",
                    @event);

            return @event.DataLockEventId;
        }
    }
}