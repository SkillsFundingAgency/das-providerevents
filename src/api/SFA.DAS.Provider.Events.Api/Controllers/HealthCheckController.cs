using System.Web.Http;
using SFA.DAS.Provider.Events.Api.Types;
using NLog;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [RoutePrefix("api/healthcheck")]
    
    public class HealthCheckController : ApiController
    {
        private readonly ILogger _logger;

        public IHttpActionResult Get()
        {
            _logger.Error("Hello World");

            return Ok(new HealthStatus());
        }
    }
}