using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPaymentStatistics
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private Client.PaymentsEventsApiClient _client;
        private Mock<SecureHttpClient> _httpClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);


            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync("http://test.local.url/api/v2/payments/statistics"))
                .ReturnsAsync(JsonConvert.SerializeObject(new PaymentStatistics()
                {
                    TotalNumberOfPayments = 500,
                    TotalNumberOfPaymentsWithRequiredPayment = 470
                }));

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, _httpClient.Object);
        }

        
        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPaymentStatistics();

            // Assert
            Assert.IsNotNull(actual);
            actual.TotalNumberOfPayments.Should().Be(500);
            actual.TotalNumberOfPaymentsWithRequiredPayment.Should().Be(470);
        }

    
    }
}
