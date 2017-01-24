using System.Web.Http;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("error")]
    public class ErrorController : ApiController
    {
        [Route("general")]
        public IHttpActionResult GeneralError()
        {
            return InternalServerError();
        }

        [Route("notfound", Name = "NotFound")]
        public IHttpActionResult NotFoundError()
        {
            return NotFound();
        }
    }
}
