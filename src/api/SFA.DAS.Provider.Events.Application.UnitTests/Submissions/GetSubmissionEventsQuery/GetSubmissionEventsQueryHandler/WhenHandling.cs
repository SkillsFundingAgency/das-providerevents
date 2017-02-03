using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryHandler
{
    public class WhenHandling
    {
        private Mock<IValidator<GetSubmissionEventsQueryRequest>> _validator;
        private Mock<ISubmissionEventsRepository> _submissionEventsRepository;
        private Application.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryHandler _handler;
        private GetSubmissionEventsQueryRequest _request;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetSubmissionEventsQueryRequest>>(); _validator.Setup(v => v.Validate(It.IsAny<GetSubmissionEventsQueryRequest>()))
                 .ReturnsAsync(new ValidationResult());

            _submissionEventsRepository = new Mock<ISubmissionEventsRepository>();
            _submissionEventsRepository.Setup(r => r.GetSubmissionEventsSinceId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Data.Entities.PageOfEntities<Domain.Data.Entities.SubmissionEventEntity>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        new Domain.Data.Entities.SubmissionEventEntity {Id = 1}
                    }
                });
            _submissionEventsRepository.Setup(r => r.GetSubmissionEventsSinceTime(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Data.Entities.PageOfEntities<Domain.Data.Entities.SubmissionEventEntity>
                {
                    PageNumber = 2,
                    TotalNumberOfPages = 3,
                    Items = new[]
                    {
                        new Domain.Data.Entities.SubmissionEventEntity {Id = 2}
                    }
                });

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<PageOfResults<SubmissionEvent>>(It.IsAny<PageOfEntities<SubmissionEventEntity>>()))
                .Returns((PageOfEntities<SubmissionEventEntity> source) =>
                {
                    return new PageOfResults<SubmissionEvent>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        Items = source.Items.Select(e => new SubmissionEvent
                        {
                            Id = e.Id
                        }).ToArray()
                    };
                });

            _handler = new Application.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryHandler(_validator.Object, _submissionEventsRepository.Object, _mapper.Object);

            _request = new GetSubmissionEventsQueryRequest();
        }

        [Test]
        public async Task ThenItShouldReturnInvalidResponseWhenValidatorFails()
        {
            // Arrange
            _validator.Setup(v => v.Validate(It.IsAny<GetSubmissionEventsQueryRequest>()))
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
            _submissionEventsRepository.Setup(r => r.GetSubmissionEventsSinceId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
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
