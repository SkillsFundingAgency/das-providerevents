using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPayments
    {
        private PaymentsEventsApiConfiguration _configuration;
        private Payment _payment1;
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

            _payment1 = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 123456,
                Uln = 987654,
                EmployerAccountId = "ACC001",
                ApprenticeshipId = 918273,
                DeliveryPeriod = new CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Month = 9,
                    Year = 2017
                },
                EvidenceSubmittedOn = new DateTime(2017, 10, 1),
                EmployerAccountVersion = "A",
                ApprenticeshipVersion = "B",
                FundingSource = FundingSource.Levy,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PageOfResults<Payment>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        _payment1
                    }
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetPayments("XXX", "YYY", 2);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/payments?page=2&periodId=XXX&employerAccountId=YYY"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPayments();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_payment1.Id, actual.Items[0].Id);
            Assert.AreEqual(_payment1.Ukprn, actual.Items[0].Ukprn);
            Assert.AreEqual(_payment1.Uln, actual.Items[0].Uln);
            Assert.AreEqual(_payment1.EmployerAccountId, actual.Items[0].EmployerAccountId);
            Assert.AreEqual(_payment1.ApprenticeshipId, actual.Items[0].ApprenticeshipId);
            Assert.AreEqual(_payment1.DeliveryPeriod.Month, actual.Items[0].DeliveryPeriod.Month);
            Assert.AreEqual(_payment1.DeliveryPeriod.Year, actual.Items[0].DeliveryPeriod.Year);
            Assert.AreEqual(_payment1.CollectionPeriod.Month, actual.Items[0].CollectionPeriod.Month);
            Assert.AreEqual(_payment1.CollectionPeriod.Year, actual.Items[0].CollectionPeriod.Year);
            Assert.AreEqual(_payment1.EvidenceSubmittedOn, actual.Items[0].EvidenceSubmittedOn);
            Assert.AreEqual(_payment1.EmployerAccountVersion, actual.Items[0].EmployerAccountVersion);
            Assert.AreEqual(_payment1.ApprenticeshipVersion, actual.Items[0].ApprenticeshipVersion);
            Assert.AreEqual(_payment1.FundingSource, actual.Items[0].FundingSource);
            Assert.AreEqual(_payment1.TransactionType, actual.Items[0].TransactionType);
            Assert.AreEqual(_payment1.Amount, actual.Items[0].Amount);
        }
    }
}
