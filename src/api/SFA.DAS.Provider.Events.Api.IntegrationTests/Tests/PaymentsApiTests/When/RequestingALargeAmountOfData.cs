using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.PaymentsApiTests.When
{
    [TestFixture()]
    public class RequestingALargeAmountOfData
    {
        [Test]
        public async Task TheNumberOfPagesIsCorrect()
        {
            // Assuming 10000 per page
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            result.TotalNumberOfPages.Should().Be(6);
        }

        [Test]
        public async Task TheNumberOfResultsIs10000()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            result.Items.Should().HaveCount(10000);
        }
    }
}
