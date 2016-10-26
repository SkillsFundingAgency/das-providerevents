using System.Web.Http;
using Microsoft.Azure;
using SFA.DAS.ApiTokens.Client;
using SFA.DAS.Payments.Events.Api.Plumbing.Json;

namespace SFA.DAS.Payments.Events.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StrictEnumConverter());

            var apiKeySecret = CloudConfigurationManager.GetSetting("ApiTokenSecret");
            var apiIssuer = CloudConfigurationManager.GetSetting("ApiIssuer");
            var apiAudiences = CloudConfigurationManager.GetSetting("ApiAudiences").Split(' ');

            config.MessageHandlers.Add(new ApiKeyHandler("Authorization", apiKeySecret, apiIssuer, apiAudiences));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
