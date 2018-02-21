using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.RecordProcessRunRecord
{
    public class WhenHandling
    {
        private Mock<IDataLockEventRepository> _dataLockEventRepository;
        private RecordProcessorRunHandler _handler;
        private RecordProcessorRunRequest _request;

        [SetUp]
        public void Arrange()
        {
            _dataLockEventRepository = new Mock<IDataLockEventRepository>();

            _handler = new RecordProcessorRunHandler(_dataLockEventRepository.Object);

            _request = new RecordProcessorRunRequest();
        }

        [Test]
        public async Task ThenItShouldReturnErrorWithInternalException()
        {
            // Arrange
            _request.Ukprn = 1;
            _request.StartTimeUtc = DateTime.Today;

            _dataLockEventRepository.Setup(r => r.InsertOrUpdateProcessRunRecord(null, 1, null, DateTime.Today, null, null, null, null))
                .Throws(new ApplicationException("test ex")).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.AreEqual("test ex", actual.Exception.Message);
        }

        [Test]
        public async Task ThenItShouldReturnValidResponseAndSetProviderProcessor()
        {
            // Arrange
            _request.Ukprn = 1;
            _request.StartTimeUtc = DateTime.Today;

            _dataLockEventRepository.Setup(r => r.InsertOrUpdateProcessRunRecord(null, 1, null, DateTime.Today, null, null, null, null)).ReturnsAsync(777).Verifiable();
            _dataLockEventRepository.Setup(r => r.SetProviderProcessor(1, 777)).Returns(Task.FromResult(default(object))).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task ThenItShouldClearProviderProcessor()
        {
            // Arrange
            _request.Ukprn = 1;
            _request.StartTimeUtc = DateTime.Today;
            _request.FinishTimeUtc = DateTime.Today;

            _dataLockEventRepository.Setup(r => r.InsertOrUpdateProcessRunRecord(null, 1, null, DateTime.Today, DateTime.Today, null, null, null)).ReturnsAsync(777).Verifiable("Processor run record was not inserted");
            _dataLockEventRepository.Setup(r => r.ClearProviderProcessor(1)).Returns(Task.FromResult(default(object))).Verifiable("Processor run record was not associated with provider");

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }
    }
}