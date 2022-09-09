using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPeriodEnds : ClientUnitTestBase
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiClientConfiguration> _configuration;
        private PeriodEnd _periodEnd1;
        private IPaymentsEventsApiClient _client;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiClientConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);

            _periodEnd1 = new PeriodEnd
            {
                Id = "1617-R01",
                CalendarPeriod = new CalendarPeriod
                {
                    Month = 9,
                    Year = 2016
                },
                ReferenceData = new ReferenceDataDetails
                {
                    AccountDataValidAt = new DateTime(2016, 9, 1),
                    CommitmentDataValidAt = new DateTime(2016, 9, 2)
                },
                CompletionDateTime = new DateTime(2016, 10, 3),
                Links = new PeriodEndLinks
                {
                    PaymentsForPeriod = "some-other-url"
                }
            };

            _httpMessageHandlerMock = SetupHttpMessageHandler(JsonConvert.SerializeObject(
                new[]
                {
                    _periodEnd1
                }));

            // use real http client with mocked handler
            var factory = new PaymentsEventsApiClientFactory(_configuration.Object);
            _client = factory.CreateClient(_httpMessageHandlerMock.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetPeriodEnds();

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/periodends");
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPeriodEnds();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(_periodEnd1.Id, actual[0].Id);
            Assert.AreEqual(_periodEnd1.CalendarPeriod.Month, actual[0].CalendarPeriod.Month);
            Assert.AreEqual(_periodEnd1.CalendarPeriod.Year, actual[0].CalendarPeriod.Year);
            Assert.AreEqual(_periodEnd1.ReferenceData.AccountDataValidAt, actual[0].ReferenceData.AccountDataValidAt);
            Assert.AreEqual(_periodEnd1.ReferenceData.CommitmentDataValidAt, actual[0].ReferenceData.CommitmentDataValidAt);
            Assert.AreEqual(_periodEnd1.CompletionDateTime, actual[0].CompletionDateTime);
            Assert.AreEqual(_periodEnd1.Links.PaymentsForPeriod, actual[0].Links.PaymentsForPeriod);
        }
    }
}
