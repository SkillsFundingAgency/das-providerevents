using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Client
{
    public interface IPaymentsEventsApiClient
    {
        /// <summary>
        /// Get list of period end that have been run
        /// </summary>
        /// <returns>Task that yields an array of PeriodEnds</returns>
        Task<PeriodEnd[]> GetPeriodEnds();

        /// <summary>
        /// Get a page of payments
        /// </summary>
        /// <param name="periodId">The period identifier to filter by, i.e. 1617-R01. Default is null for no filter</param>
        /// <param name="employerAccountId">The employer account identifier to filter by, i.e. 12345. Default is null for no filter</param>
        /// <param name="page">The page number to view. Default is 1</param>
        /// <returns>A task that yields a page of payments</returns>
        Task<PageOfResults<Payment>> GetPayments(string periodId = null, string employerAccountId = null, int page = 1);
    }
}