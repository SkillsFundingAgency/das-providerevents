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
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.TransfersController
{
    [TestFixture]
    public class WhenThingsGoWrong : BaseMockController
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
        public async Task AndInvalidPeriodIsPassedThenItShouldReturnError()
        {
            // Arrange
            var periodQueryResponse = new GetPeriodQueryResponse
            {
                IsValid = false,
                Exception = new ValidationException(new[]{"mars attacks!"})
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodQueryRequest>())).ReturnsAsync(periodQueryResponse).Verifiable("GetPeriod was never called");

            // Act
            var actual = await _controller.GetTransfers("regular");

            // Assert
            _mockMediator.VerifyAll();
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
        }

        [Test]
        public async Task AndInvalidTransferRequestCreatedSomehowThenItShouldReturnError()
        {
            // Arrange
            var periodQueryResponse = new GetPeriodQueryResponse
            {
                IsValid = true,
                Result = new CollectionPeriod()
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodQueryRequest>())).ReturnsAsync(periodQueryResponse).Verifiable("GetPeriod was never called");
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTransfersQueryRequest>())).ReturnsAsync(new GetTransfersQueryResponse
            {
                IsValid = false,
                Exception = new ValidationException(new[]{"mars attacks!"})
            }).Verifiable("GetTransfersQuery was never called");

            // Act
            var actual = await _controller.GetTransfers("regular");

            // Assert
            _mockMediator.VerifyAll();
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
        }

        [Test]
        public async Task AndExceptionOccursThenItShouldReturnErrorResult()
        {
            // Arrange
            var periodQueryResponse = new GetPeriodQueryResponse
            {
                IsValid = true,
                Result = new CollectionPeriod()
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodQueryRequest>())).ReturnsAsync(periodQueryResponse).Verifiable("GetPeriod was never called");
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTransfersQueryRequest>())).Throws<ApplicationException>().Verifiable("GetTransfersQuery was never called");

            // Act
            var actual = await _controller.GetTransfers("regular");

            // Assert
            _mockMediator.VerifyAll();
            Assert.IsNotNull(actual);
            Assert.IsNotInstanceOf<OkNegotiatedContentResult<PageOfResults<AccountTransfer>>>(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }
    }
}
