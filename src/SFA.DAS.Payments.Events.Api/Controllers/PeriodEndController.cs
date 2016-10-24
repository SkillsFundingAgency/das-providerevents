using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public Task<IHttpActionResult> ListPeriodEnds(int page = 1, int pageSize = 1000)
        {
            var result = new PageOfResults<PeriodEnd>
            {
                PageNumber = page,
                TotalNumberOfPages = pageSize,
                Items = new[]
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
                }
            };

            return Task.FromResult<IHttpActionResult>(Ok(result));
        }
    }
}
