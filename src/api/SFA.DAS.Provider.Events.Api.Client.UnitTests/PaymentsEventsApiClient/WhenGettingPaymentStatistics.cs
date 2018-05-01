using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPaymentStatistics
    {
        private PaymentsEventsApiConfiguration _configuration;
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

      
            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync( "some-url/api/payments/statistics"))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PaymentStatistics()
                {
                    TotalNumberOfPayments = 500,
                    TotalNumberOfRecievedPayments = 470
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        
        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPaymentStatisctics();

            // Assert
            Assert.IsNotNull(actual);
            actual.TotalNumberOfPayments.Should().Be(500);
            actual.TotalNumberOfRecievedPayments.Should().Be(470);
        }

    
    }
}
