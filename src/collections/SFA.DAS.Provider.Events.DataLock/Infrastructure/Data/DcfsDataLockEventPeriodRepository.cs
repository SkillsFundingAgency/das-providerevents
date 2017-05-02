using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventPeriodRepository : DcfsRepository, IDataLockEventPeriodRepository
    {
        private readonly string _connectionString;
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
            _connectionString = connectionString;
        }

        public DataLockEventPeriodEntity[] GetDataLockEventPeriods(Guid eventId)
        {
            return Query<DataLockEventPeriodEntity>(SelectEventPeriods, new { eventId });
        }

        public void BulkWriteDataLockEventPeriods(DataLockEventPeriodEntity[] periods)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < periods.Length)
                {
                    var batch = periods.Skip(skip).Take(batchSize)
                        .Select(x => $"('{x.DataLockEventId}', '{x.CollectionPeriodName}', {x.CollectionPeriodMonth}, {x.CollectionPeriodYear}, " +
                                     $"{x.CommitmentVersion}, {(x.IsPayable ? 1 : 0)}, {x.TransactionType})")
                        .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO DataLockEvents.DataLockEventPeriods " +
                                              "(DataLockEventId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, " +
                                              "CommitmentVersion, IsPayable, TransactionType) " +
                                              $"VALUES {batch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }
    }
}