using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.GetDataLockEventsQuery.GetDataLockEventsQueryHandler
{
    public class WhenHandling
    {
        private Mock<IValidator<GetDataLockEventsQueryRequest>> _validator;
        private Mock<IDataLockRepository> _dataLockEventsRepository;
        private Application.DataLock.GetDataLockEventsQuery.GetDataLockEventsQueryHandler _handler;
        private GetDataLockEventsQueryRequest _request;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetDataLockEventsQueryRequest>>();
            _validator
                .Setup(v => v.Validate(It.IsAny<GetDataLockEventsQueryRequest>()))
                .ReturnsAsync(new ValidationResult());

            _dataLockEventsRepository = new Mock<IDataLockRepository>();
            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsSinceId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 1, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsSinceTime(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 2,
                    TotalNumberOfPages = 3,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 2, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForAccountSinceId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 3,
                    TotalNumberOfPages = 4,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 3, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForAccountSinceTime(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 4,
                    TotalNumberOfPages = 5,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 4, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForProviderSinceId(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 5,
                    TotalNumberOfPages = 6,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 5, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForProviderSinceTime(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 6,
                    TotalNumberOfPages = 7,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 6, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForAccountAndProviderSinceId(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 7,
                    TotalNumberOfPages = 8,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 7, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockEventsForAccountAndProviderSinceTime(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfEntities<DataLockEventEntity>
                {
                    PageNumber = 8,
                    TotalNumberOfPages = 9,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 8, HasErrors = true }
                    }
                });

            _dataLockEventsRepository
                .Setup(r => r.GetDataLockErrorsForDataLockEvent(It.IsAny<long>()))
                .ReturnsAsync(new[]
                {
                    new DataLockEventErrorEntity
                    {
                        DataLockEventId = 1
                    }
                });

            _mapper = new Mock<IMapper>();
            _mapper
                .Setup(m => m.Map<PageOfResults<DataLockEvent>>(It.IsAny<PageOfEntities<DataLockEventEntity>>()))
                .Returns((PageOfEntities<DataLockEventEntity> source) =>
                {
                    return new PageOfResults<DataLockEvent>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        Items = source.Items.Select(e => new DataLockEvent
                        {
                            Id = e.Id
                        }).ToArray()
                    };
                });

            _handler = new Application.DataLock.GetDataLockEventsQuery.GetDataLockEventsQueryHandler(_validator.Object, _dataLockEventsRepository.Object, _mapper.Object);

            _request = new GetDataLockEventsQueryRequest();
        }

        [Test]
        public async Task ThenItShouldReturnInvalidResponseWhenValidatorFails()
        {
            // Arrange
            _validator.Setup(v => v.Validate(It.IsAny<GetDataLockEventsQueryRequest>()))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationMessages = new[] { "Invalid" }
                });

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsInstanceOf<ValidationException>(actual.Exception);
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
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountAndUkprnAndTimeIfFilterSpecified()
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.Ukprn = 1;
            _request.SinceTime = DateTime.Now;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(8, actual.Result.PageNumber);
            Assert.AreEqual(9, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(8, actual.Result.Items[0].Id);
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountAndUkprnAndEventIdIfFilterSpecified(int eventId)
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.Ukprn = 1;
            _request.SinceEventId = eventId;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(7, actual.Result.PageNumber);
            Assert.AreEqual(8, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(7, actual.Result.Items[0].Id);
        }

        [Test]
        public async Task ThenItShouldReturnResultsBasedOnUkprnAndTimeIfFilterSpecified()
        {
            // Arrange
            _request.Ukprn = 1;
            _request.SinceTime = DateTime.Now;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(6, actual.Result.PageNumber);
            Assert.AreEqual(7, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(6, actual.Result.Items[0].Id);
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task ThenItShouldReturnResultsBasedOnUkprnAndEventIdIfFilterSpecified(int eventId)
        {
            // Arrange
            _request.Ukprn = 1;
            _request.SinceEventId = eventId;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(5, actual.Result.PageNumber);
            Assert.AreEqual(6, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(5, actual.Result.Items[0].Id);
        }

        [Test]
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountAndTimeIfFilterSpecified()
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.SinceTime = DateTime.Now;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(4, actual.Result.PageNumber);
            Assert.AreEqual(5, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(4, actual.Result.Items[0].Id);
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountEventIdIfFilterSpecified(int eventId)
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.SinceEventId = eventId;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(3, actual.Result.PageNumber);
            Assert.AreEqual(4, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(3, actual.Result.Items[0].Id);
        }

        [Test]
        public async Task ThenItShouldReturnResultsBasesOnTimeIfTimeFilterSpecified()
        {
            // Arrange
            _request.SinceTime = DateTime.Now;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.PageNumber);
            Assert.AreEqual(3, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(2, actual.Result.Items[0].Id);
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task ThenItShouldReturnResultsBasesOnEventIdIfTimeFilterNotSpecified(int eventId)
        {
            // Arrange
            _request.SinceEventId = eventId;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(1, actual.Result.PageNumber);
            Assert.AreEqual(2, actual.Result.TotalNumberOfPages);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Id);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidResponseIfExceptionOccurs()
        {
            // Arrange
            _dataLockEventsRepository.Setup(r => r.GetDataLockEventsSinceId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test"));

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsInstanceOf<Exception>(actual.Exception);
        }
    }
}