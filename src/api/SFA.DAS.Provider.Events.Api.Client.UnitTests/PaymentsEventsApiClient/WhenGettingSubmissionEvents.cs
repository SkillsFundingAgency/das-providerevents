﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingSubmissionEvents : ClientUnitTestBase
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private SubmissionEvent _submissionStandardEvent;
        private SubmissionEvent _submissionFrameworkEvent;
        private Client.PaymentsEventsApiClient _client;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);

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
                PlannedEndDate = new DateTime(2018, 5, 1),
                TrainingPrice = 12000m,
                EndpointAssessorPrice = 3000m,
                NiNumber = "AB12345C",
                ApprenticeshipId = 1,
                AcademicYear = "1617",
                EmployerReferenceNumber = 123456,
                EPAOrgId = "EPACodeI"
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
                TrainingPrice = 6000m,
                EndpointAssessorPrice = 1500m,
                NiNumber = "AB12345C",
                ApprenticeshipId = 9,
                AcademicYear = "1617",
                EmployerReferenceNumber = 123456,
                EPAOrgId = "EPACodeI"
            };

            _httpMessageHandlerMock = SetupHttpMessageHandler(JsonConvert.SerializeObject(
                new PageOfResults<SubmissionEvent>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                {
                    _submissionStandardEvent,
                    _submissionFrameworkEvent
                }
                }));

            // use real http client with mocked handler
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, httpClient);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceTimeFilter()
        {
            // Act
            await _client.GetSubmissionEvents(0, new DateTime(2017, 2, 8, 12, 10, 45), 0, 7);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/submissions?page=7&sinceTime=2017-02-08T12:10:45");
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceEventIdFilter()
        {
            // Act
            await _client.GetSubmissionEvents(9, null, 0, 7);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/submissions?page=7&sinceEventId=9");
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForProviderFilter()
        {
            // Act
            await _client.GetSubmissionEvents(0, null, 123, 7);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/submissions?page=7&ukprn=123");
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetSubmissionEvents();

            // Assert
            //todo: should really clone the input data before using it as expected, in case the code under test mutates it
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
                   && original.TrainingPrice == client.TrainingPrice
                   && original.EndpointAssessorPrice == client.EndpointAssessorPrice
                   && original.NiNumber == client.NiNumber
                   && original.ApprenticeshipId == client.ApprenticeshipId
                   && original.AcademicYear == client.AcademicYear
                   && original.EmployerReferenceNumber == client.EmployerReferenceNumber
                   && original.EPAOrgId == client.EPAOrgId;
        }
    }
}