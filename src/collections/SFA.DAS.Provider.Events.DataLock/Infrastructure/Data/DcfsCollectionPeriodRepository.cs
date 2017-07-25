using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsCollectionPeriodRepository : DcfsRepository, ICollectionPeriodRepository
    {
        private const int OpenStatus = 1;

        private const string CollectionPeriodSource = "FROM Reference.CollectionPeriods";
        private const string CollectionPeriodColumns = "[Id] AS [PeriodId]," +
                                                       "[Name] AS [Name]," +
                                                       "[CalendarMonth] AS [Month]," +
                                                       "[CalendarYear] AS [Year]";
        private const string SelectCollectionPeriods = "SELECT " + CollectionPeriodColumns + " FROM " + CollectionPeriodSource;
        private const string SelectOpenCollectionPeriod = SelectCollectionPeriods + " WHERE Open = @CollectionOpen";

        public DcfsCollectionPeriodRepository(string connectionString)
            : base(connectionString)
        {
        }

        public CollectionPeriodEntity GetCurrentCollectionPeriod()
        {
            return QuerySingle<CollectionPeriodEntity>(SelectOpenCollectionPeriod, new { CollectionOpen = OpenStatus });
        }
    }
}
