using System;
using System.Linq;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.Context;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.DependencyResolution
{
    public class EventsSourcePolicy : ConfiguredInstancePolicy
    {
        private readonly ContextWrapper _contextWrapper;

        public EventsSourcePolicy(ContextWrapper contextWrapper)
        {
            _contextWrapper = contextWrapper;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var parameter = instance.Constructor.GetParameters().FirstOrDefault(x => x.Name == "eventsSource");

            if (parameter != null)
            {
                var eventsSource = GetEventSourceOrThrow(_contextWrapper.GetPropertyValue(DataLockContextPropertyKeys.DataLockEventsSource));
                instance.Dependencies.AddForConstructorParameter(parameter, eventsSource);
            }
        }

        private EventSource GetEventSourceOrThrow(string eventsSource)
        {
            EventSource source;

            if (!Enum.TryParse(eventsSource, true, out source))
            {
                throw new ArgumentException($"Invalid event source of {eventsSource} found.");
            }

            return source;
        }
    }
}