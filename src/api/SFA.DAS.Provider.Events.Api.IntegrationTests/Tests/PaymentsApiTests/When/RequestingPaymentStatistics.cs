using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.PaymentsApiTests.When
{
    [TestFixture]
    public class RequestingPaymentStatistics
    {
        [Test]
        public async Task ThenTheCountOfPaymentRecordsIsCorrect()
        {
            
            var paymentCount = TestData.Payments.Count();

            var results = await IntegrationTestServer.Client.GetAsync($"/api/v2/payments/statistics").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PaymentStatistics>(resultsAsString);

            items.TotalNumberOfPayments.Should().Be(paymentCount);
        }

        [Test]
        public async Task ThenTheCountOfPaymentsWithRequiredRecordsIsCorrect()
        {

            var requiredPaymentList = TestData.RequiredPayments.Select(x => x.Id).ToList();
            var receivedPaymentCount = TestData.Payments.Count(x => requiredPaymentList.Contains(x.RequiredPaymentId));
   

            var results = await IntegrationTestServer.Client.GetAsync($"/api/v2/payments/statistics").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PaymentStatistics>(resultsAsString);
            
            items.TotalNumberOfPaymentsWithRequired.Should().Be(receivedPaymentCount);
        }
    }
}
