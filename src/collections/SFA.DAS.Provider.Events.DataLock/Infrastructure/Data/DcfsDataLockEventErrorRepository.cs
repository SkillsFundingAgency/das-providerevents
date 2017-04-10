using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventErrorRepository : DcfsRepository, IDataLockEventErrorRepository
    {
        private const string Source = "Reference.DataLockEventErrors";
        private const string Columns = "DataLockEventId," +
                                       "ErrorCode," +
                                       "SystemDescription";
        private const string SelectEventErrors = "SELECT " + Columns + " FROM " + Source + " WHERE DataLockEventId = @eventId";

        public DcfsDataLockEventErrorRepository(string connectionString)
            : base(connectionString)
        {
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
    }
}