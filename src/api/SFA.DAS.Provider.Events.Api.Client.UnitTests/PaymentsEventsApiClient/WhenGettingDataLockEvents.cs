using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingDataLockEvents
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private DataLockEvent _dataLockEvent;
        private Client.PaymentsEventsApiClient _client;
        private Mock<SecureHttpClient> _httpClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);
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
                ApprenticeshipId = 1,
                EmployerAccountId = 123,
                EventSource = EventSource.Submission,
                HasErrors = true,
                IlrStartDate = new DateTime(2017, 5, 1),
                IlrStandardCode = 27,
                IlrTrainingPrice = 12000m,
                IlrEndpointAssessorPrice = 3000m,
                Errors = new []
                {
                    new DataLockEventError
                    {
                        ErrorCode = "Err15",
                        SystemDescription = "Mismatch on price."
                    }
                },
                Periods = new []
                {
                    new DataLockEventPeriod
                    {
                        ApprenticeshipVersion = "1-019",
                        Period = new NamedCalendarPeriod
                        {
                            Id = "1617-R09",
                            Month = 4,
                            Year = 2017
                        },
                        IsPayable = false,
                        TransactionType = TransactionType.Learning
                    },
                    new DataLockEventPeriod
                    {
                        ApprenticeshipVersion = "1-019",
                        Period = new NamedCalendarPeriod
                        {
                            Id = "1617-R10",
                            Month = 5,
                            Year = 2017
                        },
                        IsPayable = false
                    }
                },
                Apprenticeships = new []
                {
                    new DataLockEventApprenticeship
                    {
                        Version = "19",
                        StartDate = new DateTime(2017, 5, 1),
                        StandardCode = 27,
                        NegotiatedPrice = 17500m,
                        EffectiveDate = new DateTime(2017, 5, 1)
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

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceTimeFilter()
        {
            // Act
            await _client.GetDataLockEvents(0, new DateTime(2017, 2, 8, 12, 10, 45), "123", 456, 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("http://test.local.url/api/datalock?page=7&sinceTime=2017-02-08T12:10:45&employerAccountId=123&ukprn=456"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForSinceEventIdFilter()
        {
            // Act
            await _client.GetDataLockEvents(9, null, "123", 456, 7);

            // Assert
            _httpClient.Verify(c => c.GetAsync("http://test.local.url/api/datalock?page=7&sinceEventId=9&employerAccountId=123&ukprn=456"), Times.Once);
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
                   && original.ApprenticeshipId == client.ApprenticeshipId
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
                   && EventErrorsMatch(original.Errors, client.Errors)
                   && EventPeriodsMatch(original.Periods, client.Periods)
                   && EventApprenticeshipsMatch(original.Apprenticeships, client.Apprenticeships);
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
            return original.ErrorCode == client.ErrorCode
                   && original.SystemDescription == client.SystemDescription;
        }

        private bool EventPeriodsMatch(DataLockEventPeriod[] original, DataLockEventPeriod[] client)
        {
            if (original.Length != client.Length)
            {
                return false;
            }

            var result = true;

            for (var x = 0; x < original.Length; x++)
            {
                result = result && PeriodsMatch(original[x], client[x]);
            }

            return result;
        }

        private bool PeriodsMatch(DataLockEventPeriod original, DataLockEventPeriod client)
        {
            return original.ApprenticeshipVersion == client.ApprenticeshipVersion
                   && original.Period.Id == client.Period.Id
                   && original.Period.Month == client.Period.Month
                   && original.Period.Year == client.Period.Year
                   && original.IsPayable == client.IsPayable
                   && original.TransactionType == client.TransactionType;
        }

        private bool EventApprenticeshipsMatch(DataLockEventApprenticeship[] original, DataLockEventApprenticeship[] client)
        {
            if (original.Length != client.Length)
            {
                return false;
            }

            var result = true;

            for (var x = 0; x < original.Length; x++)
            {
                result = result & ApprenticeshipsMatch(original[x], client[x]);
            }

            return result;
        }

        private bool ApprenticeshipsMatch(DataLockEventApprenticeship original, DataLockEventApprenticeship client)
        {
            return original.Version == client.Version
                   && original.StartDate == client.StartDate
                   && original.StandardCode == client.StandardCode
                   && original.ProgrammeType == client.ProgrammeType
                   && original.FrameworkCode == client.FrameworkCode
                   && original.PathwayCode == client.PathwayCode
                   && original.NegotiatedPrice == client.NegotiatedPrice
                   && original.EffectiveDate == client.EffectiveDate;
        }
    }
}