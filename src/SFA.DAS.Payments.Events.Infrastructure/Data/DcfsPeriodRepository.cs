using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Domain.Data;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Infrastructure.Data
{
    public class DcfsPeriodRepository : IPeriodRepository
    {
        public Task<PeriodEntity> GetPeriod(string academicYear, string periodName)
        {
            return Task.FromResult(new PeriodEntity
            {
                Id = $"{academicYear}-{periodName}",
                CalendarMonth = 9,
                CalendarYear = 2017
            });
        }
    }
}
