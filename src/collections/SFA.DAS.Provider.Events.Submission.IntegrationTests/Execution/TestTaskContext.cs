using System.Collections.Generic;
using CS.Common.External.Interfaces;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Execution
{
    public class TestTaskContext : IExternalContext
    {
        public TestTaskContext()
        {
            Properties = new Dictionary<string, string>
            {
                { ContextPropertyKeys.TransientDatabaseConnectionString, GlobalTestContext.Current.TransientDatabaseConnectionString },
                { ContextPropertyKeys.LogLevel, "Debug" },
            };
        }

        public IDictionary<string, string> Properties { get; set; }
    }
}
