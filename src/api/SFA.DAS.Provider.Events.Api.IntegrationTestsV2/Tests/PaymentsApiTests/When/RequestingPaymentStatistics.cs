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
    public class RequestingPaymentStatistics
    {
        [Test]
        public async Task ThenTheCountOfPaymentRecordsIsCorrect()
        {

            var paymentCount = TestData.AllPayments.Count();

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/v2/payments/statistics").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PaymentStatistics>(resultsAsString);

            items.TotalNumberOfPayments.Should().Be(paymentCount);
            items.TotalNumberOfPayments.Should().NotBe(0);
        }

        [Test]
        public async Task ThenTheCountOfPaymentsWithRequiredRecordsIsCorrect()
        {
            var requiredPaymentsCount = TestData.AllPayments.Count(x => x.RequiredPaymentEventId != null);

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/v2/payments/statistics").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PaymentStatistics>(resultsAsString);

            items.TotalNumberOfPaymentsWithRequiredPayment.Should().Be(requiredPaymentsCount);
            items.TotalNumberOfPaymentsWithRequiredPayment.Should().NotBe(0);
        }
    }
}
