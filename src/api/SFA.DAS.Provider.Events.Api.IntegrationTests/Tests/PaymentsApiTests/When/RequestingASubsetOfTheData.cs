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
    public class RequestingASubsetOfTheData
    {
        [Test]
        public async Task TheNumberOfRecordsIsCorrect()
        {
            var firstPayment = TestData.RequiredPayments.First();
            var ukprn = firstPayment.Ukprn;
            var requiredPaymentList = TestData.RequiredPayments.Where(y => y.Ukprn == ukprn).Select(x => x.Id).ToList();
            var paymentCount = TestData.Payments.Count(x => requiredPaymentList.Contains(x.RequiredPaymentId));

            var results = await IntegrationTestServer.Client.GetAsync($"/api/payments?ukprn={ukprn}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            items.Items.Should().HaveCount(paymentCount);
        }
    }
}
