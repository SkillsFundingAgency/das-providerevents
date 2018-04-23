using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.TransfersApiTests
{
    [TestFixture]
    public class WhenRequestingTransfers
    {
        [Test]
        public async Task ThenTheNumberOfRecordsIsCorrect()
        {
            var transfer = TestData.Transfers.First();
            var senderId = transfer.SendingAccountId;
            var allTransfers = TestData.Transfers.Where(y => y.SendingAccountId == senderId).Select(x => x.Id).ToList();

            var results = await IntegrationTestServer.Client.GetAsync($"/api/transfers?senderAccountId={senderId}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(resultsAsString);

            items.Items.Should().HaveCount(allTransfers.Count);
        }
    }
}
