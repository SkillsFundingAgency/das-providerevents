using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Domain.Data
{
    public interface IPeriodRepository
    {
        Task<PeriodEntity[]> GetPeriods();
        Task<PeriodEntity> GetPeriod(string academicYear, string periodName);
    }
}