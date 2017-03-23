using System.Collections.Generic;
using CS.Common.External.Interfaces;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.Context;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution
{
    public class TestTaskContext : IExternalContext
    {
        public TestTaskContext(EventSource eventsSource)
        {
            var transientConnectionString = eventsSource == EventSource.Submission
                ? GlobalTestContext.Current.TransientSubmissionDatabaseConnectionString
                : GlobalTestContext.Current.TransientPeriodEndDatabaseConnectionString;

            Properties = new Dictionary<string, string>
            {
                { ContextPropertyKeys.TransientDatabaseConnectionString, transientConnectionString },
                { ContextPropertyKeys.LogLevel, "Debug" },
                { DataLockContextPropertyKeys.YearOfCollection, "1617" },
                { DataLockContextPropertyKeys.DataLockEventsSource, eventsSource.ToString() }
            };
        }

        public IDictionary<string, string> Properties { get; set; }
    }
}
