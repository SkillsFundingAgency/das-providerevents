using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Domain.Data;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Infrastructure.Data
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
            var command = $"SELECT {Columns} FROM {Source} ORDER BY CalendarYear, CalendarMonth";
            return await Query<PeriodEntity>(command);
        }
        public async Task<PeriodEntity> GetPeriod(string academicYear, string periodName)
        {
            var command = $"SELECT {Columns} FROM {Source} WHERE PeriodName=@academicYear + '-' + @periodName";
            return await QuerySingle<PeriodEntity>(command, new { academicYear, periodName });
        }
    }
}
