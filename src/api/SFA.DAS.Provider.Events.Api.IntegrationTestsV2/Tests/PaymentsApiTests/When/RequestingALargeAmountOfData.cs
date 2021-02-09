using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Tests.PaymentsApiTests.When
{
    [TestFixture()]
    public class RequestingALargeAmountOfData
    {
        [Test]
        public async Task ThenTheNumberOfPagesIsCorrect()
        {
            var expected = Math.DivRem(TestData.Payments.Count, 10000, out int remainder);
            if (remainder > 0)
                expected++;

            // Assuming 10000 per page
            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            
            result.TotalNumberOfPages.Should().Be(expected);
        }

        [Test]
        public async Task ThenTheNumberOfResultsIs10000()
        {
            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            result.Items.Should().HaveCount(10000);
        }

        [Test]
        public async Task ThenTheResultsHaveEarningsInformation()
        {
            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            var randomItem = result.Items[new Random().Next(10000)];
            
            randomItem.EarningDetails.Should().NotBeEmpty();
        }

        /// <remarks>
        /// This test will fail until you update your local database to the latest version of [PaymentsDue].[Earnings] (or start with a fresh db)
        /// The latest version is in TableSetup.sql
        /// To solve this issue of db schema updates when we don't recreate it each run,
        /// (which would be too slow because the tests populate a large amount of data into the db),
        /// we could have a table that contains the schema version and check that before each run.
        /// </remarks>
        [Test]
        public async Task ThenTheDataIsCorrect()
        {
            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/payments").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<PageOfResults<Payment>>(resultsAsString);

            for (var i = 0; i < 250; i++)
            {
                var randomReturnedPayment = result.Items[new Random().Next(10000)];
                var matchingPayment = TestData.Payments.First(x => x.EventId == Guid.Parse(randomReturnedPayment.Id));

                randomReturnedPayment.Amount.Should().Be(matchingPayment.Amount);
                randomReturnedPayment.ApprenticeshipId.Should().Be(matchingPayment.ApprenticeshipId);
                randomReturnedPayment.CollectionPeriod.Month.Should().Be(matchingPayment.CollectionPeriod.ToCalendarMonth());
                randomReturnedPayment.CollectionPeriod.Year.Should().Be(matchingPayment.GetCalendarYear());
                randomReturnedPayment.CollectionPeriod.Id.Should().Be(matchingPayment.GetPeriodId());
                ((byte)randomReturnedPayment.ContractType).Should().Be(matchingPayment.ContractType);
                randomReturnedPayment.DeliveryPeriod.Month.Should().Be(matchingPayment.DeliveryPeriod.ToCalendarMonth());
                randomReturnedPayment.DeliveryPeriod.Year.Should().Be(matchingPayment.GetDeliveryPeriodYear());
                randomReturnedPayment.EarningDetails.Count.Should().Be(1);

                var earningDetails = randomReturnedPayment.EarningDetails.Single();
                earningDetails.ActualEndDate.Should().Be(matchingPayment.EarningsActualEndDate.GetValueOrDefault());
                earningDetails.CompletionAmount.Should().Be(matchingPayment.EarningsCompletionAmount);
                earningDetails.CompletionStatus.Should().Be(matchingPayment.EarningsCompletionStatus);
                earningDetails.EndpointAssessorId.Should().BeNull();
                earningDetails.MonthlyInstallment.Should().Be(matchingPayment.EarningsInstalmentAmount);
                earningDetails.PlannedEndDate.Should().Be(matchingPayment.EarningsPlannedEndDate.GetValueOrDefault());
                earningDetails.RequiredPaymentId.Should().Be(matchingPayment.RequiredPaymentEventId.GetValueOrDefault());
                earningDetails.StartDate.Should().Be(matchingPayment.EarningsStartDate);
                earningDetails.TotalInstallments.Should().Be(matchingPayment.EarningsNumberOfInstalments);

                randomReturnedPayment.EmployerAccountId.Should().Be(matchingPayment.AccountId.ToString());
                randomReturnedPayment.EmployerAccountVersion.Should().BeNullOrEmpty();
                randomReturnedPayment.EvidenceSubmittedOn.Should().Be(matchingPayment.IlrSubmissionDateTime);
                randomReturnedPayment.FrameworkCode.Should().Be(matchingPayment.LearningAimFrameworkCode);
                randomReturnedPayment.FundingAccountId.Should().BeNull();
                ((byte)randomReturnedPayment.FundingSource).Should().Be(matchingPayment.FundingSource); //byte cast?
                randomReturnedPayment.Id.ToLower().Should().Be(matchingPayment.EventId.ToString().ToLower());
                randomReturnedPayment.PathwayCode.Should().Be(matchingPayment.LearningAimPathwayCode);
                randomReturnedPayment.ProgrammeType.Should().Be(matchingPayment.LearningAimProgrammeType);
                randomReturnedPayment.StandardCode.Should().Be(matchingPayment.LearningAimStandardCode);
                ((byte)randomReturnedPayment.TransactionType).Should().Be(matchingPayment.TransactionType); //byte cast?
                randomReturnedPayment.Ukprn.Should().Be(matchingPayment.Ukprn);
                randomReturnedPayment.Uln.Should().Be(matchingPayment.LearnerUln);
            }
        }
    }
}
