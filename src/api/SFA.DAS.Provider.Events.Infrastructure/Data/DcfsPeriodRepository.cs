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
            var command = $"SELECT {Columns} FROM {Source} ORDER BY CompletionDate";
            return await Query<PeriodEntity>(command).ConfigureAwait(false);
        }

        public async Task<PeriodEntity> GetPeriod(string periodId)
        {
            var academicYear = short.Parse(periodId.Substring(0, 4));

            var collectionPeriod = byte.Parse(periodId.Substring(6));

            var command = $"SELECT {Columns} FROM {Source} WHERE AcademicYear = @AcademicYear AND Period = @CollectionPeriod";
            return await QuerySingle<PeriodEntity>(command, new { academicYear, collectionPeriod })
                .ConfigureAwait(false);
        }
    }
}
