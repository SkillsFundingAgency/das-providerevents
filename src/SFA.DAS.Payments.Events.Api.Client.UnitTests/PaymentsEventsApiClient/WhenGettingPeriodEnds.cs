using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPeriodEnds
    {
        private PaymentsEventsApiConfiguration _configuration;
        private PeriodEnd _periodEnd1;
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

            _periodEnd1 = new Types.PeriodEnd
            {
                Id = "1617-R01",
                CalendarPeriod = new Types.CalendarPeriod
                {
                    Month = 9,
                    Year = 2016
                },
                ReferenceData = new Types.ReferenceDataDetails
                {
                    AccountDataValidAt = new DateTime(2016, 9, 1),
                    CommitmentDataValidAt = new DateTime(2016, 9, 2)
                },
                CompletionDateTime = new DateTime(2016, 10, 3),
                Links = new Types.PeriodEndLinks
                {
                    PaymentsForPeriod = "some-other-url"
                }
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new[]
                {
                    _periodEnd1
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetPeriodEnds();

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/periodends"), Times.Once);
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
