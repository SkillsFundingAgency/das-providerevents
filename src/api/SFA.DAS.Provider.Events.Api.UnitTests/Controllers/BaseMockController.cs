using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Provider.Events.Api.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers
{
    public class BaseMockController
    {
        public BaseMockController()
        {
            telemetryClient = InitializeMockTelemetryChannel();
        }

        protected TelemetryClient telemetryClient;

        private TelemetryClient InitializeMockTelemetryChannel()
        {
            // Application Insights TelemetryClient doesn't have an interface (and is sealed)
            // Create our own mock object
            MockTelemetryChannel mockTelemetryChannel = new MockTelemetryChannel();

            TelemetryConfiguration mockTelemetryConfig = new TelemetryConfiguration
            {
                TelemetryChannel = mockTelemetryChannel,
                InstrumentationKey = Guid.NewGuid().ToString(),
            };

            TelemetryClient mockTelemetryClient = new TelemetryClient(mockTelemetryConfig);

            return mockTelemetryClient;
        }
    }
}
