using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Microsoft.ApplicationInsights;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.TransfersController
{
    [TestFixture]
    public class WhenTransfersRequested : BaseMockController
    {
        private Api.Controllers.TransfersController _controller;
        private Mock<IMediator> _mockMediator;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>(MockBehavior.Strict);

            _controller = new Api.Controllers.TransfersController(telemetryClient, _mockMediator.Object);
        }

        [Test]
        public async Task ThenItShouldReturnTransfers()
        {
            // Arrange

            var periodQueryResponse = new GetPeriodQueryResponse
            {
                IsValid = true,
                Result = new CollectionPeriod()
            };

            var transfer = new AccountTransfer
            {
                TransferId = 1,
                SenderAccountId = 1,
                ReceiverAccountId = 2,
                Amount = 3,
                RequiredPaymentId = Guid.Empty,
                Type = TransferType.Levy
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodQueryRequest>())).ReturnsAsync(periodQueryResponse).Verifiable("GetPeriod was never called");
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTransfersQueryRequest>())).ReturnsAsync(new GetTransfersQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<AccountTransfer> { Items = new[] { transfer } }
            }).Verifiable("GetTransfersQuery was never called");

            // Act
            var actual = await _controller.GetTransfers("regular");

            // Assert
            _mockMediator.VerifyAll();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<AccountTransfer>>>(actual);
            Assert.AreSame(transfer, ((OkNegotiatedContentResult<PageOfResults<AccountTransfer>>)actual).Content.Items[0]);
        }

        [Test]
        public async Task AndWrongPeriodIsPassedThenItShouldReturnEmptyPage()
        {
            // Arrange
            var periodQueryResponse = new GetPeriodQueryResponse { IsValid = true };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodQueryRequest>())).ReturnsAsync(periodQueryResponse).Verifiable("GetPeriod was never called");

            // Act
            var actual = await _controller.GetTransfers("regular");

            // Assert
            _mockMediator.VerifyAll();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<AccountTransfer>>>(actual);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<PageOfResults<AccountTransfer>>)actual).Content.Items.Length);
        }

    }
}
