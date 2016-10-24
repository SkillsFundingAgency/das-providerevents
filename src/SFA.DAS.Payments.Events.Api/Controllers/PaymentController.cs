using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentController : ApiController
    {
        [Route("", Name = "PaymentsList")]
        public Task<IHttpActionResult> GetListOfPayments(string periodId = null, int page = 1, int pageSize = 1000)
        {
            return Task.FromResult<IHttpActionResult>(Ok(new
            {
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize
            }));
        }
    }
}
