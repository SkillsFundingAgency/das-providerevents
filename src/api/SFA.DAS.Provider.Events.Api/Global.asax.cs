using System;
using System.Web;
using System.Web.Http;
using SFA.DAS.Provider.Events.Infrastructure.Logging;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;
using System.Net;

namespace SFA.DAS.Provider.Events.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3; // only allow TLSV1.2 and SSL3

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
