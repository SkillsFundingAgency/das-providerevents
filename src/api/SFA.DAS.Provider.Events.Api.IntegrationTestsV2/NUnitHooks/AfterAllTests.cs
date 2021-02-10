using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    [SetUpFixture]
    public class AfterAllTests
    {
        [OneTimeTearDown]
        public async Task TearDown()
        {
            IntegrationTestServer.GetInstance().Shutdown();
        }
    }
}
