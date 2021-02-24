using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsPeriodRepository : DcfsRepository, IPeriodRepository
    {
        private const string Source = "[Payments2].[CollectionPeriod]";
        private const string Columns = "Id, "
                                      + "AcademicYear, "
                                      + "Period, "
                                      + "ReferenceDataValidationDate, "
                                      + "CompletionDate";

        public DcfsPeriodRepository() : base("PaymentsV2ConnectionString")
        {
        }
        
        public async Task<PeriodEntity[]> GetPeriods()
        {
            var command = $"SELECT {Columns} FROM {Source} ORDER BY CompletionDateTime";
            return await Query<PeriodEntity>(command).ConfigureAwait(false);
        }
        public async Task<PeriodEntity> GetPeriod(int? academicYear, int? collectionPeriod)
        {
            var command = $"SELECT {Columns} FROM {Source} WHERE AcademicYear = @AcademicYear AND p.CollectionPeriod = @CollectionPeriod";
            return await QuerySingle<PeriodEntity>(command, new {academicYear, collectionPeriod})
                .ConfigureAwait(false);
        }
    }
}
