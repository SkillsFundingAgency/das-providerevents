using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingLatestLearnerEventsByStandard
    {
        private PaymentsEventsApiConfiguration _configuration;
        private List<SubmissionEvent> _submissionEvents;
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

            _submissionEvents = new List<SubmissionEvent>
            {
                new SubmissionEvent
                {
                    Id = 1,
                    IlrFileName = "ILR-123456",
                    FileDateTime = new DateTime(2017, 2, 10, 8, 55, 23),
                    SubmittedDateTime = new DateTime(2017, 2, 10, 8, 59, 13),
                    ComponentVersionNumber = 1,
                    Ukprn = 123456,
                    Uln = 987654321,
                    StandardCode = 27,
                    ActualStartDate = new DateTime(2017, 4, 1),
                    PlannedEndDate =  new DateTime(2018, 5, 1),
                    TrainingPrice = 12000m,
                    EndpointAssessorPrice = 3000m,
                    NiNumber = "AB12345C",
                    ApprenticeshipId = 1,
                    AcademicYear = "1617",
                    EmployerReferenceNumber = 123456,
                    EPAOrgId = "EPACodeI",
                    FamilyName = "Jones",
                    GivenNames = "David"
                },
                new SubmissionEvent
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
                    TrainingPrice = 6000m,
                    EndpointAssessorPrice = 1500m,
                    NiNumber = "AB12345C",
                    ApprenticeshipId = 9,
                    AcademicYear = "1617",
                    EmployerReferenceNumber = 123456,
                    EPAOrgId = "EPACodeI",
                    FamilyName = "Jones",
                    GivenNames = "David",
                    StandardCode = 12
                }
            };
            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(_submissionEvents)));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceEventIdFilter()
        {
            // Act
            await _client.GetLatestLearnerEventByStandard(1111111111, 12345);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/submissions?uln=1111111111&sinceEventId=12345"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForUlnFilter()
        {
            // Act
            await _client.GetLatestLearnerEventByStandard(1111111111);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/submissions?uln=1111111111"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetLatestLearnerEventByStandard(1111111111, 12345);

            // Assert
            //todo: should really clone the input data before using it as expected, in case the code under test mutates it
            Assert.IsNotNull(actual);
            Assert.IsTrue(EventsMatch(_submissionEvents[0], actual[0]));
            Assert.IsTrue(EventsMatch(_submissionEvents[1], actual[1]));
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
                   && original.TrainingPrice == client.TrainingPrice
                   && original.EndpointAssessorPrice == client.EndpointAssessorPrice
                   && original.NiNumber == client.NiNumber
                   && original.ApprenticeshipId == client.ApprenticeshipId
                   && original.AcademicYear == client.AcademicYear
                   && original.EmployerReferenceNumber == client.EmployerReferenceNumber
                   && original.EPAOrgId == client.EPAOrgId
                    && original.FamilyName == client.FamilyName
                    && original.GivenNames == client.GivenNames;
        }
    }
}