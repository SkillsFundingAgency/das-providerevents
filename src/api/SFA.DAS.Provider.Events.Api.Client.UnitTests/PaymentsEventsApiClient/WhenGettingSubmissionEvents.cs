using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingSubmissionEvents
    {
        private PaymentsEventsApiConfiguration _configuration;
        private SubmissionEvent _submissionStandardEvent;
        private SubmissionEvent _submissionFrameworkEvent;
        private Client.PaymentsEventsApiClient _client;
        private Mock<SecureHttpClient> _httpClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new PaymentsEventsApiConfiguration
            {
                ApiBaseUrl = "some-url/",
                ClientToken = "super_secure_token"
            };

            _submissionStandardEvent = new SubmissionEvent
            {
                Id = 1,
                IlrFileName = "ILR-123456",
                FileDateTime = new DateTime(2017, 2, 10, 8, 55, 23),
                SubmittedDateTime = new DateTime(2017, 2, 10, 8, 59, 13),
                ComponentVersionNumber = 1,
                Ukprn = 123456,
                Uln = 987654,
                StandardCode = 27,
                ActualStartDate = new DateTime(2017, 4, 1),
                PlannedEndDate =  new DateTime(2018, 5, 1),
                OnProgrammeTotalPrice = 12000m,
                CompletionTotalPrice = 3000m,
                NiNumber = "AB12345C",
                CommitmentId = "1"
            };

            _submissionFrameworkEvent = new SubmissionEvent
            {
                Id = 1,
                IlrFileName = "ILR-123456",
                FileDateTime = new DateTime(2017, 2, 10, 8, 55, 23),
                SubmittedDateTime = new DateTime(2017, 2, 10, 8, 59, 13),
                ComponentVersionNumber = 1,
                Ukprn = 123456,
                Uln = 987654321,
                ProgrammeType = 20,
                FrameworkCode = 550,
                PathwayCode = 6,
                ActualStartDate = new DateTime(2017, 4, 1),
                PlannedEndDate = new DateTime(2018, 5, 1),
                OnProgrammeTotalPrice = 6000m,
                CompletionTotalPrice = 1500m,
                NiNumber = "AB12345C",
                CommitmentId = "9"
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PageOfResults<SubmissionEvent>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        _submissionStandardEvent,
                        _submissionFrameworkEvent
                    }
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceTimeFilter()
        {
            // Act
            await _client.GetSubmissionEvents(0, new DateTime(2017, 2, 8, 12, 10, 45), 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/submissions?page=7&sinceTime=2017-02-08T12:10:45"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceEventIdFilter()
        {
            // Act
            await _client.GetSubmissionEvents(9, null, 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/submissions?page=7&sinceEventId=9"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetSubmissionEvents();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(EventsMatch(_submissionStandardEvent, actual.Items[0]));
            Assert.IsTrue(EventsMatch(_submissionFrameworkEvent, actual.Items[1]));
        }

        private bool EventsMatch(SubmissionEvent original, SubmissionEvent client)
        {
            return original.Id == client.Id
                   && original.IlrFileName == client.IlrFileName
                   && original.FileDateTime == client.FileDateTime
                   && original.SubmittedDateTime == client.SubmittedDateTime
                   && original.ComponentVersionNumber == client.ComponentVersionNumber
                   && original.Ukprn == client.Ukprn
                   && original.Uln == client.Uln
                   && original.StandardCode == client.StandardCode
                   && original.ProgrammeType == client.ProgrammeType
                   && original.FrameworkCode == client.FrameworkCode
                   && original.PathwayCode == client.PathwayCode
                   && original.ActualStartDate == client.ActualStartDate
                   && original.PlannedEndDate == client.PlannedEndDate
                   && original.ActualEndDate == client.ActualEndDate
                   && original.OnProgrammeTotalPrice == client.OnProgrammeTotalPrice
                   && original.CompletionTotalPrice == client.CompletionTotalPrice
                   && original.NiNumber == client.NiNumber
                   && original.CommitmentId == client.CommitmentId;
        }
    }
}