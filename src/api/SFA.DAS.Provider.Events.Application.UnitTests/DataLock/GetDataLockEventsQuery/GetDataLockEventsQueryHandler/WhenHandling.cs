using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.GetDataLockEventsQuery.GetDataLockEventsQueryHandler
{
    public class WhenHandling
    {
        private Mock<IValidator<GetDataLockEventsQueryRequest>> _validator;
        private Mock<IDataLockEventRepository> _dataLockEventsRepository;
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

            _dataLockEventsRepository = new Mock<IDataLockEventRepository>();

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(null, null, null, null, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 1, HasErrors = true }
                    }

                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(1, null, null, null, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 1, HasErrors = true }
                    }

                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(null, DateTime.Today, null, null, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 2,
                    TotalNumberOfPages = 3,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 2, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(1, null, "Acc", null, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 3,
                    TotalNumberOfPages = 4,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 3, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(null, DateTime.Today, "Acc", null, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 4,
                    TotalNumberOfPages = 5,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 4, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(1, null, null, 1, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 5,
                    TotalNumberOfPages = 6,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 5, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(null, DateTime.Today, null, 1, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 6,
                    TotalNumberOfPages = 7,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 6, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(1, null, "Acc", 1, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 7,
                    TotalNumberOfPages = 8,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 7, HasErrors = true }
                    }
                });

            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(null, DateTime.Today, "Acc", 1, 1, 0))
                .ReturnsAsync(new PageOfResults<DataLockEventEntity>
                {
                    PageNumber = 8,
                    TotalNumberOfPages = 9,
                    Items = new[]
                    {
                        new DataLockEventEntity { Id = 8, HasErrors = true }
                    }
                });

            _mapper = new Mock<IMapper>();
            _mapper
                .Setup(m => m.Map<PageOfResults<DataLockEvent>>(It.IsAny<PageOfResults<DataLockEventEntity>>()))
                .Returns((PageOfResults<DataLockEventEntity> source) =>
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

            _request = new GetDataLockEventsQueryRequest { PageNumber = 1 };
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
            _request.SinceTime = DateTime.Today;

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

        [Test]
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountAndUkprnAndEventIdIfFilterSpecified()
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.Ukprn = 1;
            _request.SinceEventId = 1;

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
            _request.SinceTime = DateTime.Today;
            _request.PageSize = 0;

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

        [Test]
        public async Task ThenItShouldReturnResultsBasedOnUkprnAndEventIdIfFilterSpecified()
        {
            // Arrange
            _request.Ukprn = 1;
            _request.SinceEventId = 1;

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
            _request.SinceTime = DateTime.Today;

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

        [Test]
        public async Task ThenItShouldReturnResultsBasedOnEmployerAccountEventIdIfFilterSpecified()
        {
            // Arrange
            _request.EmployerAccountId = "Acc";
            _request.SinceEventId = 1;

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
            _request.SinceTime = DateTime.Today;

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

        [Test]
        public async Task ThenItShouldReturnResultsBasesOnEventIdIfTimeFilterNotSpecified()
        {
            // Arrange
            _request.SinceEventId = 1;

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
            _dataLockEventsRepository.Setup(r => r.GetDataLockEvents(It.IsAny<long?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<int>(), It.IsAny<int>()))
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