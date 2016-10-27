using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Domain.Data
{
    public interface IPeriodRepository
    {
        Task<PeriodEntity[]> GetPeriods();
        Task<PeriodEntity> GetPeriod(string academicYear, string periodName);
    }
}