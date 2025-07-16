

using Microsoft.ApplicationInsights.Channel;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Mocks
{
    public class MockTelemetryChannel : ITelemetryChannel
    {
        public bool? DeveloperMode { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string EndpointAddress { get; set; }
        public void Send(ITelemetry item)
        {
            // Intentionally left blank for mocking purposes
        }
        public void Flush()
        {
            // Intentionally left blank for mocking purposes
        }
        public void Dispose()
        {
            // Intentionally left blank for mocking purposes
        }
    }
}
