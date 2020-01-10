using System.Web.Http;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Plumbing.DependencyResolution;
using SFA.DAS.Provider.Events.Api.Plumbing.DependencyResolution.Policies;
using SFA.DAS.Provider.Events.Api.Plumbing.Json;
using WebApi.StructureMap;

namespace SFA.DAS.Provider.Events.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StrictEnumConverter());

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

            config.UseStructureMap(x =>
            {
                x.Policies.Add<LoggingPolicy>();
                x.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
