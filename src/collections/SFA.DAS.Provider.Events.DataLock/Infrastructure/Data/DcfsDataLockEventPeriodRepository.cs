using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventPeriodRepository : DcfsRepository, IDataLockEventPeriodRepository
    {
        private const string Source = "Reference.DataLockEventPeriods";
        private const string Columns = "DataLockEventId," +
                                       "CollectionPeriodName," +
                                       "CollectionPeriodMonth," +
                                       "CollectionPeriodYear," +
                                       "CommitmentVersion," +
                                       "IsPayable," +
                                       "TransactionType";
        private const string SelectEventPeriods = "SELECT " + Columns + " FROM " + Source + " WHERE DataLockEventId = @eventId";

        public DcfsDataLockEventPeriodRepository(string connectionString)
            : base(connectionString)
        {
        }

        public DataLockEventPeriodEntity[] GetDataLockEventPeriods(long eventId)
        {
            return Query<DataLockEventPeriodEntity>(SelectEventPeriods, new { eventId });
        }

        public void WriteDataLockEventPeriod(DataLockEventPeriodEntity period)
        {
            Execute("INSERT INTO DataLock.DataLockEventPeriods " +
                    "(DataLockEventId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, CommitmentVersion, IsPayable, TransactionType) " +
                    "VALUES " +
                    "(@DataLockEventId, @CollectionPeriodName, @CollectionPeriodMonth, @CollectionPeriodYear, @CommitmentVersion, @IsPayable, @TransactionType)",
                    period);
        }
    }
}