using System.Web.Http;
using SFA.DAS.Provider.Events.Api.Models;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [RoutePrefix("api/healthcheck")]
    public class HealthCheckController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new HealthModel());
        }
    }
}