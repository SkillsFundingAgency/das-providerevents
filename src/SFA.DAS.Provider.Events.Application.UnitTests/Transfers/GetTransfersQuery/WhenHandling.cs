using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Transfers.GetTransfersQuery
{
    [TestFixture]
    public class WhenHandling
    {
        private GetTransfersQueryHandler _getTransfersQueryHandler;
        private Mock<ITransferRepository> _mockTransferRepository;
        private Mock<IMapper> _mockMapper;

        [SetUp]
        public void SetUp()
        {
            _mockTransferRepository = new Mock<ITransferRepository>(MockBehavior.Strict);
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            _getTransfersQueryHandler = new GetTransfersQueryHandler(_mockTransferRepository.Object, _mockMapper.Object);
        }

        [Test]
        public async Task ThenItShouldReturnTransfersFromRepo()
        {
            // Arrange
            var request = new GetTransfersQueryRequest();
            var response = new GetTransfersQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<AccountTransfer>
                {
                    Items = new AccountTransfer[0]
                }
            };

            var entities = new PageOfResults<TransferEntity>();

            _mockTransferRepository.Setup(x => x.GetTransfers(request.PageNumber, request.PageSize, request.SenderAccountId, null, null, null)).ReturnsAsync(entities);
            _mockMapper.Setup(x => x.Map<PageOfResults<AccountTransfer>>(entities)).Returns(response.Result);

            // Act
            var actualResult = await _getTransfersQueryHandler.Handle(request).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsValid);
            Assert.AreSame(response.Result, actualResult.Result);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidResponseIfSomethingWrong()
        {
            // Arrange
            var request = new GetTransfersQueryRequest();
            var response = new GetTransfersQueryResponse
            {
                IsValid = false,
                Exception = new ApplicationException()
            };

            _mockTransferRepository.Setup(x => x.GetTransfers(request.PageNumber, request.PageSize, request.SenderAccountId, null, null, null)).Throws(response.Exception);

            // Act
            var actualResult = await _getTransfersQueryHandler.Handle(request).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(actualResult.IsValid);
            Assert.AreSame(response.Exception, actualResult.Exception);
        }
    }
}
