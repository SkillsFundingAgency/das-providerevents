using System;
using System.Linq;
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
        public async Task ThenTheNumberOfPagesIsCorrect()
        {
            var expected = TestData.Payments.Count / 10000;

            // Assuming 10000 per page
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            
            result.TotalNumberOfPages.Should().Be(expected);
        }

        [Test]
        public async Task ThenTheNumberOfResultsIs10000()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            result.Items.Should().HaveCount(10000);
        }

        [Test]
        public async Task ThenTheResultsHaveEarningsInformation()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            var randomItem = result.Items[new Random().Next(10000)];
            
            randomItem.EarningDetails.Should().NotBeEmpty();
        }

        [Test]
        public async Task ThenTheDataIsCorrect()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            var randomItem = result.Items[new Random().Next(10000)];
            var matchingPayment = TestData.Payments.First(x => x.PaymentId == Guid.Parse(randomItem.Id));

            var matchingEarnings = TestData.Earnings
                .Where(x => x.RequiredPaymentId == matchingPayment.RequiredPaymentId)
                .Select(x => new Earning
                {
                    ActualEndDate = x.ActualEndDate,
                    CompletionAmount = x.CompletionAmount,
                    CompletionStatus = x.CompletionStatus,
                    MonthlyInstallment = x.MonthlyInstallment,
                    PlannedEndDate = x.PlannedEndDate,
                    StartDate = x.StartDate,
                    TotalInstallments = x.TotalInstallments,
                });

            randomItem.EarningDetails.ShouldAllBeEquivalentTo(matchingEarnings, 
                options => options.Excluding(x => x.RequiredPaymentId));
        }
    }
}
