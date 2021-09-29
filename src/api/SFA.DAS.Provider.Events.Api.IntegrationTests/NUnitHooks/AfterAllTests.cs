using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests
{
    [SetUpFixture]
    public class AfterAllTests
    {
        [OneTimeTearDown]
        public Task TearDown()
        {
            IntegrationTestServer.Shutdown();
            return Task.CompletedTask;
        }
    }
}
