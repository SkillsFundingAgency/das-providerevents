using System;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client
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

        /// <summary>
        /// Get a page of submissions
        /// </summary>
        /// <param name="sinceEventId">An event id to read from (non-inclusive)</param>
        /// <param name="sinceTime">A time to read from (non-inclusive)</param>
        /// <param name="page">The page number to view. Default is 1</param>
        /// <returns>A task that yields a page of submission events</returns>
        Task<PageOfResults<SubmissionEvent>> GetSubmissionEvents(int sinceEventId = 0, DateTime? sinceTime = null, int page = 1);
    }
}