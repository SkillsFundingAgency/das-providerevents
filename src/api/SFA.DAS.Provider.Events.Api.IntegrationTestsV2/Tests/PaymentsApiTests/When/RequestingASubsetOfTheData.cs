using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Tests.PaymentsApiTests.When
{
    [TestFixture]
    public class RequestingASubsetOfTheData
    {
        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrect()
        {
            var ukprn = TestData.Payments.First().Ukprn;
            var paymentCount = TestData.Payments.Count(x => x.Ukprn == ukprn);

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/payments?ukprn={ukprn}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            items.Items.Should().HaveCount(paymentCount);
            items.Items.Length.Should().NotBe(0);
        }
    }
}
