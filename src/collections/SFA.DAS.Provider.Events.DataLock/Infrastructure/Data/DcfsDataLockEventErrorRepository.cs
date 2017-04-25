using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventErrorRepository : DcfsRepository, IDataLockEventErrorRepository
    {
        private readonly string _connectionString;
        private const string Source = "Reference.DataLockEventErrors";
        private const string Columns = "DataLockEventId," +
                                       "ErrorCode," +
                                       "SystemDescription";
        private const string SelectEventErrors = "SELECT " + Columns + " FROM " + Source + " WHERE DataLockEventId = @eventId";

        public DcfsDataLockEventErrorRepository(string connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public DataLockEventErrorEntity[] GetDatalockEventErrors(Guid eventId)
        {
            return Query<DataLockEventErrorEntity>(SelectEventErrors, new { eventId });
        }

        public void WriteDataLockEventError(DataLockEventErrorEntity error)
        {
            Execute("INSERT INTO DataLockEvents.DataLockEventErrors " +
                    "(DataLockEventId, ErrorCode, SystemDescription) " +
                    "VALUES " +
                    "(@DataLockEventId, @ErrorCode, @SystemDescription)",
                    error);
        }

        public void BulkWriteDataLockEventError(DataLockEventErrorEntity[] errors)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < errors.Length)
                {
                    var batch = errors.Skip(skip).Take(batchSize)
                        .Select(x => $"('{x.DataLockEventId}', '{x.ErrorCode}', '{x.SystemDescription}')")
                        .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO DataLockEvents.DataLockEventErrors " +
                                              "(DataLockEventId, ErrorCode, SystemDescription) " +
                                              $"VALUES {batch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }
    }
}