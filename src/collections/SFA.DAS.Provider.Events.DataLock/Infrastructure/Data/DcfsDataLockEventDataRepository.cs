using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventDataRepository : DcfsRepository, IDataLockEventDataRepository
    {
        public DcfsDataLockEventDataRepository(string connectionString)
            : base(connectionString)
        {
        }

        public DataLockEventDataEntity[] GetCurrentEvents(long ukprn)
        {
            return Query<DataLockEventDataEntity>("EXEC DataLockEvents.GetCurrentEventData @Ukprn", new { ukprn });
        }
    }
}
