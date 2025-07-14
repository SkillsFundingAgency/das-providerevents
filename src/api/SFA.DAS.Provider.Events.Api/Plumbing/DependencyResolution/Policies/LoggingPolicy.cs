using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Provider.Events.Api.Plumbing.DependencyResolution.Policies
{
    public class LoggingPolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var telemetryClientParam = instance?.Constructor?.GetParameters()
                .FirstOrDefault(x => x.ParameterType == typeof(TelemetryClient));

            if (telemetryClientParam != null)
            {
                // You may want to use a singleton TelemetryClient, or resolve it from your IoC container
                var telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);
                instance.Dependencies.AddForConstructorParameter(telemetryClientParam, telemetryClient);
            }
        }
    }
}