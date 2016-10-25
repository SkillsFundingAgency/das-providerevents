using System;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/periodend")]
    public class PeriodEndController : ApiController
    {

        [Route("", Name = "PeriodEndList")]
        [HttpGet]
        public Task<IHttpActionResult> ListPeriodEnds()
        {
            var result = new[]
            {
                new PeriodEnd
                {
                    Id = "0917",
                    CalendarPeriod = new CalendarPeriod
                    {
                        Month = 9,
                        Year = 2017
                    },
                    ReferenceData = new ReferenceDataDetails
                    {
                        AccountDataValidTill = new DateTime(2017, 9, 30),
                        CommitmentDataValidTill = new DateTime(2017, 10, 1)
                    },
                    RunOn = new DateTime(2017,10, 5, 19, 30, 0),
                    Links = new PeriodEndLinks
                    {
                        PaymentsForPeriod = Url.Link("PaymentsList", new { periodId = "0917" })
                    }
                }
            };

            return Task.FromResult<IHttpActionResult>(Ok(result));
        }
    }
}
