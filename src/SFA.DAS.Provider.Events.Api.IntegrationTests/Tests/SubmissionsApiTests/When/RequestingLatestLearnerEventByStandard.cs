using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.SubmissionsApiTests.When
{
    [TestFixture]
    public class RequestingLatestLearnerEventByStandard
    {
        [Test]
        public async Task ThenTheEventDataIsCorrect()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/learners?sinceEventId=0&uln=1002105691").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<List<SubmissionEvent>>(resultsAsString);

            Assert.IsNotNull(events);
            events.Count.Should().Be(2);
        }
    }
}
