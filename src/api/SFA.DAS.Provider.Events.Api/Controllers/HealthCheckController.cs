using System.Web.Http;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [RoutePrefix("api/healthcheck")]
    public class HealthCheckController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new HealthStatus());
        }
    }
}