using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingTransfers
    {
        private PaymentsEventsApiConfiguration _configuration;
        private AccountTransfer _transfer;
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

            _transfer = new AccountTransfer
            {
                TransferId = 1,
                SenderAccountId = 666,
                ReceiverAccountId = 777,
                Type = TransferType.Levy,
                RequiredPaymentId = Guid.NewGuid(),
                Amount = 888
            };

            _httpClient = new Mock<SecureHttpClient>(MockBehavior.Strict);
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(new PageOfResults<AccountTransfer>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        _transfer
                    }
                })));

            _client = new Client.PaymentsEventsApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetTransfers("XXX", 222, null, 2);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/transfers?page=2&periodId=XXX&senderAccountId=222"), Times.Once);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForReceiverAccountIdFilter()
        {
            // Act
            await _client.GetTransfers("XXX", 333, 444, 2);

            // Assert
            _httpClient.Verify(c => c.GetAsync("some-url/api/transfers?page=2&periodId=XXX&senderAccountId=333&receiverAccountId=444"), Times.Once);
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
