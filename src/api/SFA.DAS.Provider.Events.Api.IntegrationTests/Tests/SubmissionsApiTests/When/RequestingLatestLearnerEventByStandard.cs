using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.SubmissionsApiTests.When
{
    [TestFixture]
    public class RequestingLatestLearnerEventByStandard
    {
        [Test]
        public async Task WithUlnFilter_ThenTheEventDataIsCorrect()
        {
            var uln = 1002105691;
            var results = await IntegrationTestServer.Client.GetAsync($"/api/learners?uln={uln}").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(resultsAsString);

            Assert.IsNotNull(events);
            events.Items.All(i => i.Uln == uln).Should().BeTrue();
            events.Items.Count().Should().Be(2);
        }

        [Test]
        public async Task WithoutUlnFilter_ThenTheEventDataIsCorrect()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/learners").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(resultsAsString);

            Assert.IsNotNull(events);
            events.Items.Any(i => i.Uln == 0).Should().BeFalse();
            events.Items.Count().Should().Be(3);
        }
    }
}
