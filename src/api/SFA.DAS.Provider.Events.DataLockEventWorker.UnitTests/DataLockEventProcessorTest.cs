using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.DataLock.UpdateProviderQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.UnitTests
{
    [TestFixture]
    public class DataLockEventProcessorTest
    {
        private IDataLockProcessor _dataLockProcessor;
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        private GetProvidersQueryResponse _getProvidersQueryResponse;

        [SetUp]
        public void SetUp()
        {
            _dataLockProcessor = new DataLockProcessor(_loggerMock.Object, _mediatorMock.Object);
            _getProvidersQueryResponse = new GetProvidersQueryResponse
            {
                IsValid = true,
                Result = new[] {new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today}}
            };
        }

        [Test]
        public async Task TestProcessorRequestsNewDataLocks()
        {
            // arrange
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetCurrentDataLocksQueryRequest>())).ReturnsAsync(new GetCurrentDataLocksQueryResponse { IsValid = true }).Verifiable("Current Data Locks were not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetLatestDataLocksQueryRequest>())).ReturnsAsync(new GetLatestDataLocksQueryResponse(){ IsValid = true }).Verifiable("Latest Data Locks were not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<UpdateProviderQueryRequest>(r => r.Provider.IlrSubmissionDateTime == DateTime.Today))).ReturnsAsync(new UpdateProviderQueryResponse {IsValid = true}).Verifiable("Provider update was not requested");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }

        [Test]
        public async Task TestDataLockComparisonProducesEvents()
        {
            // arrange
            var currentDataLocksQueryResponse = new GetCurrentDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock>
                {
                    Items = new[]
                    {
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" } },// existing unchanged
                        new DataLock {Ukprn = 1, AimSequenceNumber = 2, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" } },// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 3, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1"},// new
                        //new DataLock {Ukprn = 1, AimSequenceNumber = 4, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1"} // old
                    }
                }
            };

            var lastDataLocksQueryResponse = new GetLatestDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock>
                {
                    Items = new[]
                    {
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" }},// existing unchanged
                        new DataLock {Ukprn = 1, AimSequenceNumber = 2, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", Commitments = new List<long> { 1 }},// existing changed
                        //new DataLock {Ukprn = 1, AimSequenceNumber = 3, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" }},// new
                        new DataLock {Ukprn = 1, AimSequenceNumber = 4, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" }} // old
                    }
                }
            };

            WriteDataLocksQueryRequest actualWriteDataLocksRequest = null;
            WriteDataLockEventsQueryRequest actualWriteDataLockEventsRequest = null;
            
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(currentDataLocksQueryResponse).Verifiable("Current Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 2))).ReturnsAsync(new GetCurrentDataLocksQueryResponse{ IsValid = true}).Verifiable("Current Data Locks page 2 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(lastDataLocksQueryResponse).Verifiable("Latest Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 2))).ReturnsAsync(new GetLatestDataLocksQueryResponse{ IsValid = true}).Verifiable("Latest Data Locks page 2 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<UpdateProviderQueryRequest>(r => r.Provider.IlrSubmissionDateTime == DateTime.Today))).ReturnsAsync(new UpdateProviderQueryResponse {IsValid = true}).Verifiable("Provider update was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<WriteDataLocksQueryRequest>()))
                .ReturnsAsync(new WriteDataLocksQueryResponse {IsValid = true })
                .Callback<WriteDataLocksQueryRequest>(r => actualWriteDataLocksRequest = r)
                .Verifiable("Write new Data Locks was not called");

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<WriteDataLockEventsQueryRequest>()))
                .ReturnsAsync(new WriteDataLockEventsQueryResponse {IsValid = true})
                .Callback<WriteDataLockEventsQueryRequest>(r => actualWriteDataLockEventsRequest = r)
                .Verifiable("Write new Data Lock Events was not called");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();

            Assert.IsNotNull(actualWriteDataLocksRequest);

            // 1 new data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.NewDataLocks);
            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks[0].Ukprn);
            Assert.AreEqual(3, actualWriteDataLocksRequest.NewDataLocks[0].AimSequenceNumber);

            // 1 updated data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.UpdatedDataLocks);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[0].Ukprn);
            Assert.AreEqual(2, actualWriteDataLocksRequest.UpdatedDataLocks[0].AimSequenceNumber);

            // 1 deleted data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.RemovedDataLocks);
            Assert.AreEqual(1, actualWriteDataLocksRequest.RemovedDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.RemovedDataLocks[0].Ukprn);
            Assert.AreEqual(4, actualWriteDataLocksRequest.RemovedDataLocks[0].AimSequenceNumber);

            // 3 events
            Assert.IsNotNull(actualWriteDataLockEventsRequest);
            Assert.AreEqual(3, actualWriteDataLockEventsRequest.DataLockEvents.Count);
        }


        [Test]
        public async Task TestDataLockComparisonOnVariousPrpoerties()
        {
            // arrange
            var currentDataLocksQueryResponse = new GetCurrentDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock>
                {
                    Items = new[]
                    {
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" } },// existing unchanged
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" } },// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "4", PriceEpisodeIdentifier = "4", ErrorCodes = new List<string> { "E1" } },// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "5", PriceEpisodeIdentifier = "5", Commitments = new []{ 1L }},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "6", PriceEpisodeIdentifier = "6"},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "7", PriceEpisodeIdentifier = "7", Commitments = new []{ 1L, 5L, 7L }},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "2"},// new
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "3", PriceEpisodeIdentifier = "2"},// new
                        //new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "3"} // old
                    }
                }
            };

            var lastDataLocksQueryResponse = new GetLatestDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock>
                {
                    Items = new[]
                    {
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "1", PriceEpisodeIdentifier = "1", ErrorCodes = new List<string> { "E1" }},// existing unchanged
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "1", Commitments = new List<long> { 1 }},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "4", PriceEpisodeIdentifier = "4", ErrorCodes = new List<string> { "E1", "E2" }},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "5", PriceEpisodeIdentifier = "5"},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "6", PriceEpisodeIdentifier = "6", Commitments = new []{ 1L }},// existing changed
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "7", PriceEpisodeIdentifier = "7", Commitments = new []{ 1L, 2L, 3L }},// existing changed
                        //new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "2", ErrorCodes = new List<string> { "E1" }},// new
                        new DataLock {Ukprn = 1, AimSequenceNumber = 1, LearnerReferenceNumber = "2", PriceEpisodeIdentifier = "3", ErrorCodes = new List<string> { "E1" }} // old
                    }
                }
            };

            WriteDataLocksQueryRequest actualWriteDataLocksRequest = null;
            WriteDataLockEventsQueryRequest actualWriteDataLockEventsRequest = null;
            
            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(_getProvidersQueryResponse).Verifiable("Provider list was not requested");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(currentDataLocksQueryResponse).Verifiable("Current Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 2))).ReturnsAsync(new GetCurrentDataLocksQueryResponse{ IsValid = true}).Verifiable("Current Data Locks page 2 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 1))).ReturnsAsync(lastDataLocksQueryResponse).Verifiable("Latest Data Locks page 1 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetLatestDataLocksQueryRequest>(r => r.PageNumber == 2))).ReturnsAsync(new GetLatestDataLocksQueryResponse{ IsValid = true}).Verifiable("Latest Data Locks page 2 was not requested for provider");
            _mediatorMock.Setup(m => m.SendAsync(It.Is<UpdateProviderQueryRequest>(r => r.Provider.IlrSubmissionDateTime == DateTime.Today))).ReturnsAsync(new UpdateProviderQueryResponse {IsValid = true}).Verifiable("Provider update was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<WriteDataLocksQueryRequest>()))
                .ReturnsAsync(new WriteDataLocksQueryResponse {IsValid = true })
                .Callback<WriteDataLocksQueryRequest>(r => actualWriteDataLocksRequest = r)
                .Verifiable("Write new Data Locks was not called");

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<WriteDataLockEventsQueryRequest>()))
                .ReturnsAsync(new WriteDataLockEventsQueryResponse {IsValid = true})
                .Callback<WriteDataLockEventsQueryRequest>(r => actualWriteDataLockEventsRequest = r)
                .Verifiable("Write new Data Lock Events was not called");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();

            Assert.IsNotNull(actualWriteDataLocksRequest);

            // 2 new data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.NewDataLocks);
            Assert.AreEqual(2, actualWriteDataLocksRequest.NewDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks[0].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks[0].AimSequenceNumber);
            Assert.AreEqual("2", actualWriteDataLocksRequest.NewDataLocks[0].LearnerReferenceNumber);
            Assert.AreEqual("2", actualWriteDataLocksRequest.NewDataLocks[0].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks[1].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.NewDataLocks[1].AimSequenceNumber);
            Assert.AreEqual("3", actualWriteDataLocksRequest.NewDataLocks[1].LearnerReferenceNumber);
            Assert.AreEqual("2", actualWriteDataLocksRequest.NewDataLocks[1].PriceEpisodeIdentifier);

            // 5 updated data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.UpdatedDataLocks);
            Assert.AreEqual(5, actualWriteDataLocksRequest.UpdatedDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[0].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[0].AimSequenceNumber);
            Assert.AreEqual("2", actualWriteDataLocksRequest.UpdatedDataLocks[0].LearnerReferenceNumber);
            Assert.AreEqual("1", actualWriteDataLocksRequest.UpdatedDataLocks[0].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[1].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[1].AimSequenceNumber);
            Assert.AreEqual("4", actualWriteDataLocksRequest.UpdatedDataLocks[1].LearnerReferenceNumber);
            Assert.AreEqual("4", actualWriteDataLocksRequest.UpdatedDataLocks[1].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[2].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[2].AimSequenceNumber);
            Assert.AreEqual("5", actualWriteDataLocksRequest.UpdatedDataLocks[2].LearnerReferenceNumber);
            Assert.AreEqual("5", actualWriteDataLocksRequest.UpdatedDataLocks[2].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[3].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[3].AimSequenceNumber);
            Assert.AreEqual("6", actualWriteDataLocksRequest.UpdatedDataLocks[3].LearnerReferenceNumber);
            Assert.AreEqual("6", actualWriteDataLocksRequest.UpdatedDataLocks[3].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[4].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.UpdatedDataLocks[4].AimSequenceNumber);
            Assert.AreEqual("7", actualWriteDataLocksRequest.UpdatedDataLocks[4].LearnerReferenceNumber);
            Assert.AreEqual("7", actualWriteDataLocksRequest.UpdatedDataLocks[4].PriceEpisodeIdentifier);

            // 1 deleted data lock
            Assert.IsNotNull(actualWriteDataLocksRequest.RemovedDataLocks);
            Assert.AreEqual(1, actualWriteDataLocksRequest.RemovedDataLocks.Count);
            Assert.AreEqual(1, actualWriteDataLocksRequest.RemovedDataLocks[0].Ukprn);
            Assert.AreEqual(1, actualWriteDataLocksRequest.RemovedDataLocks[0].AimSequenceNumber);
            Assert.AreEqual("2", actualWriteDataLocksRequest.RemovedDataLocks[0].LearnerReferenceNumber);
            Assert.AreEqual("3", actualWriteDataLocksRequest.RemovedDataLocks[0].PriceEpisodeIdentifier);

            // 3 events
            Assert.IsNotNull(actualWriteDataLockEventsRequest);
            Assert.AreEqual(8, actualWriteDataLockEventsRequest.DataLockEvents.Count);
        }

        [Test]
        public async Task TestProcessGracefullyEndsWhenNoProviders()
        {
            // arrange
            var providersQueryResponse = new GetProvidersQueryResponse
            {
                IsValid = true,
                Result = new ProviderEntity[0]
            };

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>())).ReturnsAsync(providersQueryResponse).Verifiable("Provider list was not requested");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();
        }        
        
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
