using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Tests.PeriodApiTests.When
{
    [TestFixture]
    public class RequestingPeriodData
    {
        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrect()
        {
            var periodCount = await TestData.GetPeriodCount();

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/periodends").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PeriodEnd[]>(resultsAsString);

            items.Length.Should().NotBe(0);
            items.Length.Should().Be(periodCount);
        }
    }
}
