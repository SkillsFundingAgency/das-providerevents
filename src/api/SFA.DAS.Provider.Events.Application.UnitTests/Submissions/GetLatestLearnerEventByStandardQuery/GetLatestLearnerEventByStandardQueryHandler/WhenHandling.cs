using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetLatestLearnerEventByStandardQuery.GetLatestLearnerEventByStandardQueryHandler
{
    public class WhenHandling
    {
        private Mock<IValidator<GetLatestLearnerEventForStandardsQueryRequest>> _validator;
        private Mock<ISubmissionEventsRepository> _submissionEventsRepository;
        private GetLatestLearnerEventForStandardsQueryHandler _handler;
        private GetLatestLearnerEventForStandardsQueryRequest _request;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetLatestLearnerEventForStandardsQueryRequest>>();
            _validator.Setup(v => v.Validate(It.IsAny<GetLatestLearnerEventForStandardsQueryRequest>()))
                .ReturnsAsync(new ValidationResult());

            _submissionEventsRepository = new Mock<ISubmissionEventsRepository>();
            var _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<List<SubmissionEvent>>(It.IsAny<IEnumerable<SubmissionEventEntity>>()))
                .Returns((IEnumerable<SubmissionEventEntity> source) =>
                {
                    return source.ToList().Select(e => new SubmissionEvent
                    {
                        Id = e.Id
                    }).ToList();
                });
            _handler = new GetLatestLearnerEventForStandardsQueryHandler(
                _validator.Object, _submissionEventsRepository.Object, _mapper.Object);

            _request = new GetLatestLearnerEventForStandardsQueryRequest(){SinceEventId = 22, Uln = 12345678};
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseWhenValidatorFails()
        {
            _validator.Setup(v => v.Validate(It.IsAny<GetLatestLearnerEventForStandardsQueryRequest>()))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationMessages = new[] { "Invalid" }
                });

            // Act
            var actual = _handler.Handle(_request).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsInstanceOf<ValidationException>(actual.Exception);
        }

        [Test]
        public void ThenItShouldReturnValidResults()
        {
            _submissionEventsRepository.Setup(r => r.GetLatestLearnerEventByStandard(22, 12345678))
                .ReturnsAsync(new List<SubmissionEventEntity>
                {
                    new SubmissionEventEntity() {Id = 1},
                    new SubmissionEventEntity() {Id = 2}
                });

            // Act
            var actual = _handler.Handle(_request).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.AreEqual(2, actual.Result.Count());
        }
    }
}