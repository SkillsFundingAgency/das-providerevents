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
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PaymentStatistics()
                {
                    TotalPayments = 500
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetPaymentStatisctics();

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/payments/statistics"), Times.Once);
        }
        
        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPaymentStatisctics();

            // Assert
            Assert.IsNotNull(actual);
            actual.TotalPayments.Should().Be(500);
        }

    
    }
}
