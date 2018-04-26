using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.TransfersApiTests
{
    [TestFixture]
    public class WhenRequestingTransfers
    {
        [Test]
        [TestCase("CollectionPeriodName", "periodId")]
        [TestCase("SendingAccountId", "senderAccountId")]
        [TestCase("ReceivingAccountId", "receiverAccountId")]
        public async Task ThenTheNumberOfRecordsIsCorrect(string propertyName, string apiParameterName)
        {
            var propertyInfo = typeof(ItTransfer).GetProperty(propertyName);
            
            var transfer = TestData.Transfers.First();
            var propertyValue = GetValue(propertyInfo, transfer);
            var allTransfers = TestData.Transfers.Where(y => GetValue(propertyInfo, y) == propertyValue).Select(x => x.RequiredPaymentId).ToList();

            var results = await IntegrationTestServer.Client.GetAsync($"/api/transfers?{apiParameterName}={propertyValue}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(allTransfers.Count);
        }

        private static string GetValue(PropertyInfo propertyInfo, ItTransfer transfer)
        {
            return (propertyInfo.GetValue(transfer) ?? string.Empty).ToString();
        }

        [Test]
        [TestCase("SendingAccountId", "senderAccountId", "ReceivingAccountId", "receiverAccountId")]
        [TestCase("CollectionPeriodName", "periodId", "SendingAccountId", "senderAccountId")]
        [TestCase("CollectionPeriodName", "periodId", "SendingAccountId", "senderAccountId", "ReceivingAccountId", "receiverAccountId")]
        public async Task ThenTheNumberOfRecordsIsCorrectWhenUsingTwoParameters(params object[] properties)
        {
            var transfer = TestData.Transfers.First();

            var propertyInfos = new List<PropertyInfo>();
            var propertyValues = new List<string>();
            var queryString = new List<string>();

            for (var i = 0; i < properties.Length; i += 2)
            {
                var property = typeof(ItTransfer).GetProperty(properties[i].ToString());
                var apiParameterName = properties[i + 1].ToString();
                var propertyValue = GetValue(property, transfer);

                propertyInfos.Add(property);
                propertyValues.Add(propertyValue);

                queryString.Add($"{apiParameterName}={propertyValue}");
            }
            
            var allTransfers = TestData.Transfers.Where(y =>
            {
                for (var k = 0; k < propertyInfos.Count; k++)
                {
                    var propertyInfo = propertyInfos[k];
                    if (GetValue(propertyInfo, y) != propertyValues[k])
                        return false;
                }
                return true;
            }).Select(x => x.RequiredPaymentId).ToList();

            var results = await IntegrationTestServer.Client.GetAsync("/api/transfers?" + string.Join("&", queryString)).ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(allTransfers.Count);
        }

        [Test]
        public async Task ThenPagesReturnedCorrectly()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/transfers?page=1").ConfigureAwait(false);
            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(1);
            items.Items.Should().HaveCount(10000);

            results = await IntegrationTestServer.Client.GetAsync("/api/transfers?page=6").ConfigureAwait(false);
            resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(6);
            items.Items.Should().HaveCount(10000);

            results = await IntegrationTestServer.Client.GetAsync("/api/transfers?page=7").ConfigureAwait(false);
            resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);
            items.PageNumber.Should().Be(7);
            items.Items.Should().HaveCount(0);
        }
    }
}
