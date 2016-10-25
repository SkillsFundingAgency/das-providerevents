using System;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentController : ApiController
    {
        [Route("", Name = "PaymentsList")]
        public Task<IHttpActionResult> GetListOfPayments(string periodId = null, int page = 1, int pageSize = 1000)
        {
            var result = new PageOfResults<Payment>
            {
                PageNumber = 1,
                TotalNumberOfPages = pageSize,
                Items = new[]
                {
                    new Payment
                    {
                        Id = Guid.NewGuid().ToString(),
                        Ukprn = 123456,
                        Uln = 987654,
                        EmployerAccountId = "147852",
                        ApprenticeshipId = 963258,
                        CollectionPeriod = new CalendarPeriod
                        {
                            Month = 9,
                            Year = 2017
                        },
                        DeliveryPeriod = new CalendarPeriod
                        {
                            Month = 9,
                            Year = 2017
                        },
                        EvidenceSubmittedOn = new DateTime(2017, 10, 2),
                        EmployerAccountVersion = "20170930",
                        ApprenticeshipVersion = "5",
                        FundingSource = FundingSource.Levy,
                        TransactionType = TransactionType.Learning,
                        Amount = 1000m
                    }
                }
            };
            return Task.FromResult<IHttpActionResult>(Ok(result));
        }
    }
}
