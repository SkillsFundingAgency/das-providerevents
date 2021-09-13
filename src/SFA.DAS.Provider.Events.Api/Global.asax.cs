using System;
using System.Web;
using System.Web.Http;
using SFA.DAS.Provider.Events.Infrastructure.Logging;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;

namespace SFA.DAS.Provider.Events.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application?.Context != null)
            {
                application.Context.Response.Headers.Remove("Server");
            }
        }
    }
}
