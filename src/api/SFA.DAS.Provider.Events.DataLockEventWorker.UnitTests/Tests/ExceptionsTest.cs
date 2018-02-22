using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.UnitTests.Tests
{
    [TestFixture]
    public class ExceptionsTest : DataLockProcessorTestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            
            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.FinishTimeUtc == null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Verifiable("Process start was not recorded");
            
            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.FinishTimeUtc != null && r.RunId == 777)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Verifiable("Process finish was not recorded");
        }

        [Test]
        public async Task TestProcessGracefullyEndsWhenCurrentDataLockRequestFailed()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ThrowsAsync(new ApplicationException()).Verifiable("Current data locks were never requested");

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
