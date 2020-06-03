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
    public class WhenGettingTransfers : ClientUnitTestBase
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private AccountTransfer _transfer;
        private Client.PaymentsEventsApiClient _client;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);

            _transfer = new AccountTransfer
            {
                TransferId = 1,
                SenderAccountId = 666,
                ReceiverAccountId = 777,
                Type = TransferType.Levy,
                RequiredPaymentId = Guid.NewGuid(),
                Amount = 888
            };

            _httpMessageHandlerMock = SetupHttpMessageHandler(JsonConvert.SerializeObject(new PageOfResults<AccountTransfer>
            {
                PageNumber = 1,
                TotalNumberOfPages = 2,
                Items = new[]
                {
                    _transfer
                }
            }));

            // use real http client with mocked handler
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, httpClient);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetTransfers("XXX", 222, null, 2);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/transfers?page=2&periodId=XXX&senderAccountId=222");
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForReceiverAccountIdFilter()
        {
            // Act
            await _client.GetTransfers("XXX", 333, 444, 2);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/transfers?page=2&periodId=XXX&senderAccountId=333&receiverAccountId=444");

        }


        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetTransfers();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(TransfersMatch(_transfer, actual.Items[0]));
        }

        private bool TransfersMatch(AccountTransfer original, AccountTransfer client)
        {
            return original.TransferId == client.TransferId
                   && original.SenderAccountId == client.SenderAccountId
                   && original.ReceiverAccountId == client.ReceiverAccountId
                   && original.Amount == client.Amount
                   && original.Type == client.Type
                   && original.RequiredPaymentId == client.RequiredPaymentId;
        }
    }
}
