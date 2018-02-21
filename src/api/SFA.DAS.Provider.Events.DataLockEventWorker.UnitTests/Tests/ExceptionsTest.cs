using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.UnitTests.Tests
{
    [TestFixture]
    public class ExceptionsTest : DataLockProcessorTestBase
    {

        [Test]
        public async Task TestProcessGracefullyEndsWhenCurrentDataLockRequestFailed()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ThrowsAsync(new ApplicationException()).Verifiable();

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }
        
        [Test]
        public async Task TestProcessGracefullyEndsWhenLatestDataLockRequestFails()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(new GetCurrentDataLocksQueryResponse{ IsValid = true}).Verifiable("Current Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 1))).ThrowsAsync(new ApplicationException()).Verifiable();

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }        

        [Test]
        public async Task TestProcessGracefullyEndsWhenCurrentDataLockRequestInvalid()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(new GetCurrentDataLocksQueryResponse {IsValid = false, Exception = new ApplicationException()}).Verifiable();

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }
        
        [Test]
        public async Task TestProcessGracefullyEndsWhenLatestDataLockRequestInvalid()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(new GetCurrentDataLocksQueryResponse{ IsValid = true}).Verifiable("Current Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(new GetLatestDataLocksQueryResponse {IsValid = false, Exception = new ApplicationException()}).Verifiable();

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }
    }
}
