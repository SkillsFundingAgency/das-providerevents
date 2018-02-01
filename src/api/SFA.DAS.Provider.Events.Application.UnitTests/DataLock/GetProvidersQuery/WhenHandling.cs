using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.GetProvidersQuery
{
    public class WhenHandling
    {
        private Mock<IValidator<GetProvidersQueryRequest>> _validator;
        private Mock<IDataLockEventRepository> _dataLockEventsRepository;
        private Mock<IDataLockRepository> _dataLockRepository;
        private GetProvidersQueryHandler _handler;
        private GetProvidersQueryRequest _request;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetProvidersQueryRequest>>();
            _validator
                .Setup(v => v.Validate(It.IsAny<GetProvidersQueryRequest>()))
                .ReturnsAsync(new ValidationResult());

            _dataLockRepository = new Mock<IDataLockRepository>();

            _dataLockEventsRepository = new Mock<IDataLockEventRepository>();
            
            _handler = new GetProvidersQueryHandler(_dataLockRepository.Object, _dataLockEventsRepository.Object);

            _request = new GetProvidersQueryRequest();
        }

        [Test]
        public async Task ThenItShouldReturnValidResponseWithValidatorDoesNotFail()
        {
            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task AndRequestedAllProvidersThenAllShouldReturn()
        {
            // Arrange
            _request.UpdatedOnly = false;
            var result = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today}
            };

            _dataLockRepository.Setup(r => r.GetProviders()).ReturnsAsync(result).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Count);
            Assert.AreEqual(1, actual.Result[0].Ukprn);
            Assert.AreEqual(DateTime.Today, actual.Result[0].IlrSubmissionDateTime);
        }

        [Test]
        public async Task AndRequestedUpdatedProvidersThenOnlyUpdatedShouldReturn()
        {
            // Arrange
            _request.UpdatedOnly = true;
            var result1 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today.AddDays(1)} // this one was submitted later
            };

            _dataLockRepository.Setup(r => r.GetProviders()).ReturnsAsync(result1).Verifiable("Providers were not requested from DEDS");

            var result2 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today}
            };

            _dataLockEventsRepository.Setup(r => r.GetProviders()).ReturnsAsync(result2).Verifiable("Providers were not requested from Data Lock Event storage");

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(1, actual.Result.Count);
            Assert.AreEqual(2, actual.Result[0].Ukprn);
            Assert.AreEqual(DateTime.Today.AddDays(1), actual.Result[0].IlrSubmissionDateTime);
        }

        [Test]
        public async Task AndRequestedUpdatedProvidersThenOnlyUpdatedShouldReturnInMultiplePages()
        {
            // Arrange
            _request.UpdatedOnly = true;

            var dedsPage1 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today}, // will not be returned
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today.AddDays(1)}, // will be returned
                new ProviderEntity {Ukprn = 3, IlrSubmissionDateTime = DateTime.Today}, // will not be returned
                new ProviderEntity {Ukprn = 4, IlrSubmissionDateTime = DateTime.Today.AddDays(1)} // will be returned
            };

            _dataLockRepository.Setup(r => r.GetProviders()).ReturnsAsync(dedsPage1).Verifiable("Providers page 1 were not requested from DEDS");
 
            var dlePage1 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 3, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 4, IlrSubmissionDateTime = DateTime.Today}
            };

            _dataLockEventsRepository.Setup(r => r.GetProviders()).ReturnsAsync(dlePage1).Verifiable("Providers page 1 were not requested from Data Lock Event storage");

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Count);
            Assert.AreEqual(2, actual.Result[0].Ukprn);
            Assert.AreEqual(4, actual.Result[1].Ukprn);
            Assert.AreEqual(DateTime.Today.AddDays(1), actual.Result[0].IlrSubmissionDateTime);
            Assert.AreEqual(DateTime.Today.AddDays(1), actual.Result[1].IlrSubmissionDateTime);
        }

        [Test]
        public async Task AndRequestedUpdatedProvidersThenOnlyUpdatedShouldReturnInMultiplePagesEvenIfNothingProcessed()
        {
            // Arrange
            _request.UpdatedOnly = true;

            var dedsPage1 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today}, // will not be returned
                new ProviderEntity {Ukprn = 2, IlrSubmissionDateTime = DateTime.Today.AddDays(1)}, // will be returned
                new ProviderEntity {Ukprn = 3, IlrSubmissionDateTime = DateTime.Today.AddDays(1)}, // will be returned
                new ProviderEntity {Ukprn = 4, IlrSubmissionDateTime = DateTime.Today.AddDays(1)} // will be returned but removed later to fit the page size
            };

            _dataLockRepository.Setup(r => r.GetProviders()).ReturnsAsync(dedsPage1).Verifiable("Providers page 1 were not requested from DEDS");

            var dlePage1 = new List<ProviderEntity>
            {
                new ProviderEntity {Ukprn = 5, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 6, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 7, IlrSubmissionDateTime = DateTime.Today},
                new ProviderEntity {Ukprn = 8, IlrSubmissionDateTime = DateTime.Today}
            };

            _dataLockEventsRepository.Setup(r => r.GetProviders()).ReturnsAsync(dlePage1).Verifiable("Providers page 1 were not requested from Data Lock Event storage");

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(4, actual.Result.Count);
            Assert.AreEqual(1, actual.Result[0].Ukprn);
            Assert.AreEqual(4, actual.Result[3].Ukprn);
        }
    }
}