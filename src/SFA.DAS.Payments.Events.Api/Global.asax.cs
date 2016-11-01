using System.Web.Http;
using SFA.DAS.Payments.Events.Infrastructure.Logging;

namespace SFA.DAS.Payments.Events.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
