using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingDataLockEvents
    {
        private PaymentsEventsApiConfiguration _configuration;
        private DataLockEvent _dataLockEvent;
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

            _dataLockEvent = new DataLockEvent
            {
                Id = 1,
                ProcessDateTime = new DateTime(2017, 2, 8, 9, 10, 11),
                IlrFileName = "ILR-123456",
                Ukprn = 123456,
                Uln = 987654,
                LearnRefNumber = "Lrn1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                CommitmentId = 1,
                CommitmentVersion = 19,
                EmployerAccountId = 123,
                EventSource = EventSource.Submission,
                HasErrors = true,
                IlrStartDate = new DateTime(2017, 5, 1),
                IlrStandardCode = 27,
                IlrTrainingPrice = 12000m,
                IlrEndpointAssessorPrice = 3000m,
                CommitmentStartDate = new DateTime(2017, 5, 1),
                CommitmentStandardCode = 27,
                CommitmentNegotiatedPrice = 17500m,
                CommitmentEffectiveDate = new DateTime(2017, 5, 1),
                Errors = new []
                {
                    new DataLockEventError
                    {
                        DataLockEventId = 1,
                        ErrorCode = "Err15",
                        SystemDescription = "Mismatch on price."
                    }
                }
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PageOfResults<DataLockEvent>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        _dataLockEvent
                    }
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceTimeFilter()
        {
            // Act
            await _client.GetDataLockEvents(0, new DateTime(2017, 2, 8, 12, 10, 45), "123", 456, 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/datalock?page=7&sinceTime=2017-02-08T12:10:45&employerAccountId=123&ukprn=456"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceEventIdFilter()
        {
            // Act
            await _client.GetDataLockEvents(9, null, "123", 456, 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/datalock?page=7&sinceEventId=9&employerAccountId=123&ukprn=456"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetDataLockEvents();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(EventsMatch(_dataLockEvent, actual.Items[0]));
        }

        private bool EventsMatch(DataLockEvent original, DataLockEvent client)
        {
            return original.Id == client.Id
                   && original.ProcessDateTime == client.ProcessDateTime
                   && original.IlrFileName == client.IlrFileName
                   && original.Ukprn == client.Ukprn
                   && original.Uln == client.Uln
                   && original.LearnRefNumber == client.LearnRefNumber
                   && original.AimSeqNumber == client.AimSeqNumber
                   && original.PriceEpisodeIdentifier == client.PriceEpisodeIdentifier
                   && original.CommitmentId == client.CommitmentId
                   && original.CommitmentVersion == client.CommitmentVersion
                   && original.EmployerAccountId == client.EmployerAccountId
                   && original.EventSource == client.EventSource
                   && original.HasErrors == client.HasErrors
                   && original.IlrStartDate == client.IlrStartDate
                   && original.IlrStandardCode == client.IlrStandardCode
                   && original.IlrProgrammeType == client.IlrProgrammeType
                   && original.IlrFrameworkCode == client.IlrFrameworkCode
                   && original.IlrPathwayCode == client.IlrPathwayCode
                   && original.IlrTrainingPrice == client.IlrTrainingPrice
                   && original.IlrEndpointAssessorPrice == client.IlrEndpointAssessorPrice
                   && original.CommitmentStartDate == client.CommitmentStartDate
                   && original.CommitmentStandardCode == client.CommitmentStandardCode
                   && original.CommitmentProgrammeType == client.CommitmentProgrammeType
                   && original.CommitmentFrameworkCode == client.CommitmentFrameworkCode
                   && original.CommitmentPathwayCode == client.CommitmentPathwayCode
                   && original.CommitmentNegotiatedPrice == client.CommitmentNegotiatedPrice
                   && original.CommitmentEffectiveDate == client.CommitmentEffectiveDate
                   && EventErrorsMatch(original.Errors, client.Errors);
        }

        private bool EventErrorsMatch(DataLockEventError[] original, DataLockEventError[] client)
        {
            if (original.Length != client.Length)
            {
                return false;
            }

            var result = true;

            for (var x = 0; x < original.Length; x++)
            {
                result = result && ErrorsMatch(original[x], client[x]);
            }

            return result;
        }

        private bool ErrorsMatch(DataLockEventError original, DataLockEventError client)
        {
            return original.DataLockEventId == client.DataLockEventId
                   && original.ErrorCode == client.ErrorCode
                   && original.SystemDescription == client.SystemDescription;
        }
    }
}