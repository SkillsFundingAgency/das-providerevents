using System.Web.Http;
using Microsoft.Azure;
using Newtonsoft.Json;
using SFA.DAS.ApiTokens.Client;
using SFA.DAS.Provider.Events.Api.Plumbing.Json;

namespace SFA.DAS.Provider.Events.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StrictEnumConverter());

            ConfigureJwtSecurity(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CatchAll",
                routeTemplate: "{path}",
                defaults: new { controller = "Error", action = "NotFound", path = RouteParameter.Optional }
            );
        }

        private static void ConfigureJwtSecurity(HttpConfiguration config)
        {
            var apiKeySecret = CloudConfigurationManager.GetSetting("ApiTokenSecret");
            var apiIssuer = CloudConfigurationManager.GetSetting("ApiIssuer");
            var apiAudiences = CloudConfigurationManager.GetSetting("ApiAudiences").Split(' ');

            config.MessageHandlers.Add(new ApiKeyHandler("Authorization", apiKeySecret, apiIssuer, apiAudiences));
        }
    }
}
