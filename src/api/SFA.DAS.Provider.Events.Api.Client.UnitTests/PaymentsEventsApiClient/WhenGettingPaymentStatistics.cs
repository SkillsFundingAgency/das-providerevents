using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPaymentStatistics : ClientUnitTestBase
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private Client.PaymentsEventsApiClient _client;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);

            _httpMessageHandlerMock = SetupHttpMessageHandler(JsonConvert.SerializeObject(
                new PaymentStatistics()
                {
                    TotalNumberOfPayments = 500,
                    TotalNumberOfPaymentsWithRequiredPayment = 470
                }));

            // use real http client with mocked handler
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, httpClient);
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
