using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetHistoricDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun;
using SFA.DAS.Provider.Events.Application.DataLock.UpdateProviderQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.UnitTests.Tests
{
    [TestFixture]
    public class InitialRunTest : DataLockProcessorTestBase
    {
        [Test]
        public async Task TestInitialRunDoesNotCreateNewEvents()
        {
            // arrange
            var provider1 = new ProviderEntity
            {
                Ukprn = 1,
                IlrSubmissionDateTime = DateTime.Today,
                RequiresInitialImport = true
            };
            var provider2 = new ProviderEntity
            {
                Ukprn = 2,
                IlrSubmissionDateTime = DateTime.Today,
                RequiresInitialImport = true
            };

            _getProvidersQueryResponse = new GetProvidersQueryResponse
            {
                IsValid = true,
                Result = new List<ProviderEntity> {provider1, provider2}
            };

            var dataLock1 = new DataLock
            {
                Ukprn = 1,
                AimSequenceNumber = 1,
                CommitmentId = 1,
                PriceEpisodeIdentifier = "1",
                Uln = 1
            };

            var dataLock2 = new DataLock
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                CommitmentId = 2,
                PriceEpisodeIdentifier = "2",
                Uln = 2
            };

            var dataLockEvent1 = new DataLockEvent
            {
                Ukprn = 1,
                ApprenticeshipId = 1,
                PriceEpisodeIdentifier = "1",
                Uln = 1,
                HasErrors = false,
                Status = EventStatus.Updated
            };

            var dataLockEvent2 = new DataLockEvent
            {
                Ukprn = 1,
                ApprenticeshipId = 2,
                PriceEpisodeIdentifier = "2",
                Uln = 2,
                HasErrors = true,
                Errors = new []{ new DataLockEventError { ErrorCode = "E1" }, new DataLockEventError { ErrorCode = "E2" } },
                Status = EventStatus.Removed
            };

            var currentDataLocksQueryResponse = new GetCurrentDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock> { Items = new[] { dataLock1, dataLock2 } }
            };

            var emptyCurrentDataLocksQueryResponse = new GetCurrentDataLocksQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLock> {Items = new DataLock[0]}
            };

            var historicEventsResponse = new GetHistoricDataLockEventsQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLockEvent> {Items = new[] {dataLockEvent1, dataLockEvent2}}
            };

            var emptyHistoricEventsResponse = new GetHistoricDataLockEventsQueryResponse
            {
                IsValid = true,
                Result = new PageOfResults<DataLockEvent> {Items = new[] {dataLockEvent1, dataLockEvent2}}
            };

            IList<DataLock> actualDataLocks = null;
            IList<DataLockEvent> actualDataLockEvents = null;
            ProviderEntity actualProvider1 = null;
            ProviderEntity actualProvider2 = null;
            RecordProcessorRunRequest actualRecord1 = null;
            RecordProcessorRunRequest actualRecord2 = null;
            RecordProcessorRunRequest actualRecord3 = null;
            RecordProcessorRunRequest actualRecord4 = null;

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>()))
                .ReturnsAsync(_getProvidersQueryResponse)
                .Verifiable("Provider list was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.Ukprn == 1 && r.FinishTimeUtc == null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Callback<RecordProcessorRunRequest>(r => actualRecord1 = r)
                .Verifiable("Process start for provider 1 was not recorded");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.Ukprn == 2 && r.FinishTimeUtc == null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 888, IsValid = true})
                .Callback<RecordProcessorRunRequest>(r => actualRecord3 = r)
                .Verifiable("Process start for provider 2 was not recorded");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1 && r.Ukprn == 1)))
                .ReturnsAsync(currentDataLocksQueryResponse)
                .Verifiable("Current Data Locks were not requested for provider 1");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1 && r.Ukprn == 2)))
                .ReturnsAsync(emptyCurrentDataLocksQueryResponse)
                .Verifiable("Current Data Locks were not requested for provider 2");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetHistoricDataLockEventsQueryRequest>(r => r.PageNumber == 1 && r.Ukprn == 1)))
                .ReturnsAsync(historicEventsResponse)
                .Verifiable("Historic Data Lock Events were not requested for provider 1");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetHistoricDataLockEventsQueryRequest>(r => r.PageNumber == 1 && r.Ukprn == 2)))
                .ReturnsAsync(emptyHistoricEventsResponse)
                .Verifiable("Historic Data Lock Events were not requested for provider 2");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<UpdateProviderQueryRequest>(r => r.Provider.Ukprn == 1)))
                .ReturnsAsync(new UpdateProviderQueryResponse {IsValid = true})
                .Callback<UpdateProviderQueryRequest>(p => actualProvider1 = p.Provider)
                .Verifiable("Provider 1 update was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<UpdateProviderQueryRequest>(r => r.Provider.Ukprn == 2)))
                .ReturnsAsync(new UpdateProviderQueryResponse {IsValid = true})
                .Callback<UpdateProviderQueryRequest>(p => actualProvider2 = p.Provider)
                .Verifiable("Provider 2 update was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.RunId == 777 && r.FinishTimeUtc != null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Callback<RecordProcessorRunRequest>(r => actualRecord2 = r)
                .Verifiable("Process finish for provider 1 was not recorded");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.RunId == 888 && r.FinishTimeUtc != null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 888, IsValid = true})
                .Callback<RecordProcessorRunRequest>(r => actualRecord4 = r)
                .Verifiable("Process finish for provider 2 was not recorded");

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<WriteDataLocksQueryRequest>()))
                .ReturnsAsync(new WriteDataLocksQueryResponse {IsValid = true })
                .Callback<WriteDataLocksQueryRequest>(r =>
                {
                    actualDataLocks = r.NewDataLocks ?? actualDataLocks;
                    actualDataLockEvents = r.DataLockEvents ?? actualDataLockEvents;
                })
                .Verifiable("Write new Data Locks was not called");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();

            Assert.IsNotNull(actualRecord1);
            Assert.AreEqual(1, actualRecord1.Ukprn);
            Assert.AreEqual(true, actualRecord1.IsInitialRun);
            Assert.IsNotNull(actualRecord1.StartTimeUtc);
            Assert.IsNull(actualRecord1.FinishTimeUtc);
            Assert.IsNull(actualRecord1.IsSuccess);

            Assert.IsNotNull(actualRecord2);
            Assert.AreEqual(777, actualRecord2.RunId);
            Assert.AreEqual(1, actualRecord2.Ukprn);
            Assert.IsTrue(actualRecord2.IsInitialRun);
            Assert.IsNull(actualRecord2.StartTimeUtc);
            Assert.IsNotNull(actualRecord2.FinishTimeUtc);
            Assert.IsTrue(actualRecord2.IsSuccess);

            Assert.IsNotNull(actualRecord3);
            Assert.AreEqual(2, actualRecord3.Ukprn);
            Assert.AreEqual(true, actualRecord3.IsInitialRun);
            Assert.IsNotNull(actualRecord3.StartTimeUtc);
            Assert.IsNull(actualRecord3.FinishTimeUtc);
            Assert.IsNull(actualRecord3.IsSuccess);

            Assert.IsNotNull(actualRecord4);
            Assert.AreEqual(888, actualRecord4.RunId);
            Assert.AreEqual(2, actualRecord4.Ukprn);
            Assert.IsTrue(actualRecord4.IsInitialRun);
            Assert.IsNull(actualRecord4.StartTimeUtc);
            Assert.IsNotNull(actualRecord4.FinishTimeUtc);
            Assert.IsTrue(actualRecord4.IsSuccess);

            Assert.IsNotNull(actualProvider1);
            Assert.AreEqual(1, actualProvider1.Ukprn);
            Assert.AreEqual(DateTime.Today, actualProvider1.IlrSubmissionDateTime);
            Assert.AreEqual(false, actualProvider1.RequiresInitialImport);

            Assert.IsNotNull(actualProvider2);
            Assert.AreEqual(2, actualProvider2.Ukprn);
            Assert.AreEqual(DateTime.Today, actualProvider2.IlrSubmissionDateTime);
            Assert.AreEqual(false, actualProvider2.RequiresInitialImport);

            Assert.IsNotNull(actualDataLocks);
            Assert.AreEqual(2, actualDataLocks.Count);
            Assert.AreEqual(1, actualDataLocks[0].Ukprn);
            Assert.AreEqual(1, actualDataLocks[0].AimSequenceNumber);
            Assert.AreEqual(1, actualDataLocks[0].CommitmentId);
            Assert.AreEqual("1", actualDataLocks[0].PriceEpisodeIdentifier);

            Assert.AreEqual(1, actualDataLocks[1].Ukprn);
            Assert.AreEqual(2, actualDataLocks[1].AimSequenceNumber);
            Assert.AreEqual(2, actualDataLocks[1].CommitmentId);
            Assert.AreEqual("2", actualDataLocks[1].PriceEpisodeIdentifier);

            Assert.IsNotNull(actualDataLockEvents);
            Assert.AreEqual(2, actualDataLockEvents.Count);
            Assert.AreEqual(1, actualDataLockEvents[0].Ukprn);
            Assert.AreEqual(1, actualDataLockEvents[0].Uln);
            Assert.AreEqual(1, actualDataLockEvents[0].ApprenticeshipId);
            Assert.AreEqual("1", actualDataLockEvents[0].PriceEpisodeIdentifier);
            Assert.AreEqual(EventStatus.Updated, actualDataLockEvents[0].Status);
                               
            Assert.AreEqual(1, actualDataLockEvents[1].Ukprn);
            Assert.AreEqual(2, actualDataLockEvents[1].Uln);
            Assert.AreEqual(2, actualDataLockEvents[1].ApprenticeshipId);
            Assert.AreEqual("2", actualDataLockEvents[1].PriceEpisodeIdentifier);
            Assert.AreEqual(EventStatus.Removed, actualDataLockEvents[1].Status);
        }

        [Test]
        public async Task TestInitialRunRecordsErrors()
        {
            // arrange
            var provider1 = new ProviderEntity
            {
                Ukprn = 1,
                IlrSubmissionDateTime = DateTime.Today,
                RequiresInitialImport = true
            };
            _getProvidersQueryResponse = new GetProvidersQueryResponse
            {
                IsValid = true,
                Result = new List<ProviderEntity> {provider1}
            };

            RecordProcessorRunRequest actualRequest = null;

            _mediatorMock.Setup(m => m.SendAsync(It.IsAny<GetProvidersQueryRequest>()))
                .ReturnsAsync(_getProvidersQueryResponse)
                .Verifiable("Provider list was not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.Ukprn == 1 && r.FinishTimeUtc == null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Verifiable("Process start was not recorded");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<GetCurrentDataLocksQueryRequest>(r => r.PageNumber == 1 && r.Ukprn == 1)))
                .ThrowsAsync(new ApplicationException("Test exception"))
                .Verifiable("Current Data Locks were not requested");

            _mediatorMock.Setup(m => m.SendAsync(It.Is<RecordProcessorRunRequest>(r => r.RunId == 777 && r.FinishTimeUtc != null)))
                .ReturnsAsync(new RecordProcessorRunResponse {RunId = 777, IsValid = true})
                .Callback<RecordProcessorRunRequest>(r => actualRequest = r)
                .Verifiable("Process finish for was not recorded");

            // act
            await _dataLockProcessor.ProcessDataLocks();
            
            // assert
            _mediatorMock.VerifyAll();

            Assert.IsFalse(actualRequest.IsSuccess);
            Assert.IsNotNull(actualRequest.Error);
        }
    }
}
