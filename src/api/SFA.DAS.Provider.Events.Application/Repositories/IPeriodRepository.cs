using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface IPeriodRepository
    {
        Task<PeriodEntity[]> GetPeriods();
        Task<PeriodEntity> GetPeriod(string academicYear, string periodName);
    }
}