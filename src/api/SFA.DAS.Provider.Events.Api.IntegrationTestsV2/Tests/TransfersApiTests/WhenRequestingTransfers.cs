using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Tests.TransfersApiTests
{
    [TestFixture]
    public class WhenRequestingTransfers
    {
        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrectWhenNotFiltering()
        {
            var totalExpected = TestData.TransferPayments.Count;
            var expectedPages = (int)Math.Ceiling((double)totalExpected / 10000);
            var lastPageCount = totalExpected % 10000;

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?page={expectedPages}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(lastPageCount);
        }

        [Test]
        public async Task ThenTheDataIsReturnedCorrectly()
        {
            var totalExpected = TestData.TransferPayments.Count;
            var expectedPages = (int)Math.Ceiling((double)totalExpected / 10000);

            var expectedTransfer = TestData.TransferPayments.First();

            var resultTransfers = new List<AccountTransfer>();

            for (int i = 0; i < expectedPages; i++)
            {
                var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?page={i+1}").ConfigureAwait(false);

                var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
                var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

                resultTransfers.AddRange(items.Items);
            }

            resultTransfers.Should().Contain(x =>
                x.RequiredPaymentId == expectedTransfer.RequiredPaymentEventId
                && x.Amount == expectedTransfer.Amount
                && x.CollectionPeriodName == $"{expectedTransfer.AcademicYear}-R{expectedTransfer.CollectionPeriod.ToString().PadLeft(2, '0')}"
                && x.CommitmentId == expectedTransfer.ApprenticeshipId
                && x.ReceiverAccountId == expectedTransfer.AccountId
                && x.SenderAccountId == expectedTransfer.TransferSenderAccountId
                && x.Type == TransferType.Levy
            );
        }

        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrectWhenFilteringByTransferSender()
        {
            var senderAccountId = TestData.TransferPayments.First().TransferSenderAccountId;

            var totalExpected = TestData.TransferPayments.Count(x => x.TransferSenderAccountId == senderAccountId);
            var expectedPages = (int)Math.Ceiling((double)totalExpected / 10000);
            var lastPageCount = totalExpected % 10000;

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?senderAccountId={senderAccountId}&page={expectedPages}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(lastPageCount);
        }

        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrectWhenFilteringByCollectionPeriod()
        {
            var totalExpected = TestData.TransferPayments.Count(x => x.AcademicYear == TestData.AcademicYear && x.CollectionPeriod == TestData.CollectionPeriod);
            var expectedPages = (int)Math.Ceiling((double)totalExpected / 10000);
            var lastPageCount = totalExpected % 10000;

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?periodId={TestData.AcademicYear}-R{TestData.CollectionPeriod.ToString().PadLeft(2, '0')}&page={expectedPages}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(lastPageCount);
        }

        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrectWhenFilteringByReceiverAccountId()
        {
            var receiverAccountId = TestData.TransferPayments.First().AccountId;

            var totalExpected = TestData.TransferPayments.Count(x => x.AccountId == receiverAccountId);
            var expectedPages = (int)Math.Ceiling((double)totalExpected / 10000);
            var lastPageCount = totalExpected % 10000;

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?receiverAccountId={receiverAccountId}&page={expectedPages}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(lastPageCount);
        }

        [Test]
        public async Task ThenPagesReturnedCorrectly()
        {
            var totalTransfers = TestData.TransferPayments.Count;
            var expectedPages = (int)Math.Ceiling((double)totalTransfers / 10000);
            var lastPageCount = totalTransfers % 10000;

            var results = await IntegrationTestServer.GetInstance().Client.GetAsync("/api/transfers?page=1").ConfigureAwait(false);
            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(1);
            items.Items.Should().HaveCount(Math.Min(10000, totalTransfers));

            results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?page={expectedPages-1}").ConfigureAwait(false);
            resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(expectedPages-1);
            items.Items.Should().HaveCount(10000);

            results = await IntegrationTestServer.GetInstance().Client.GetAsync($"/api/transfers?page={expectedPages}").ConfigureAwait(false);
            resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(expectedPages);
            items.Items.Should().HaveCount(lastPageCount);
        }
    }
}
