using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsPeriodRepository : DcfsRepository, IPeriodRepository
    {
        private const string Source = "Payments.Periods";
        private const string Columns = "PeriodName [Id], "
                                     + "CalendarMonth, "
                                     + "CalendarYear, "
                                     + "AccountDataValidAt, "
                                     + "CommitmentDataValidAt, "
                                     + "CompletionDateTime";

        public async Task<PeriodEntity[]> GetPeriods()
        {
            var command = $"SELECT {Columns} FROM {Source} ORDER BY CompletionDateTime";
            return await Query<PeriodEntity>(command);
        }
        public async Task<PeriodEntity> GetPeriod(string academicYear, string periodName)
        {
            var command = $"SELECT {Columns} FROM {Source} WHERE PeriodName=@academicYear + '-' + @periodName";
            return await QuerySingle<PeriodEntity>(command, new { academicYear, periodName });
        }
    }
}
