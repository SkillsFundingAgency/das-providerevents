using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetSubmissionEventsByUlnQuery.GetSubmissionEventsByUlnQueryHandler
{
    public class WhenHandling
    {
        private Mock<IValidator<GetSubmissionEventsByUlnQueryRequest>> _validator;
        private Mock<ISubmissionEventsRepository> _submissionEventsRepository;
        private Application.Submissions.GetSubmissionEventsByUlnQuery.GetSubmissionEventsByUlnQueryHandler _handler;
        private GetSubmissionEventsByUlnQueryRequest _request;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetSubmissionEventsByUlnQueryRequest>>();
            _validator.Setup(v => v.Validate(It.IsAny<GetSubmissionEventsByUlnQueryRequest>()))
                .ReturnsAsync(new ValidationResult());

            _submissionEventsRepository = new Mock<ISubmissionEventsRepository>();

            _handler = new Application.Submissions.GetSubmissionEventsByUlnQuery.GetSubmissionEventsByUlnQueryHandler(
                _validator.Object, _submissionEventsRepository.Object);

            _request = new GetSubmissionEventsByUlnQueryRequest();
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseWhenValidatorFails()
        {
            _validator.Setup(v => v.Validate(It.IsAny<GetSubmissionEventsByUlnQueryRequest>()))
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
        public void ThenItShouldReturnInvalidResponseWhenAnExceptionIsThrown()
        {
            _validator.Setup(v => v.Validate(It.IsAny<GetSubmissionEventsByUlnQueryRequest>()))
                .Throws<Exception>();

            // Act
            var actual = _handler.Handle(_request).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsInstanceOf<Exception>(actual.Exception);
        }

        [Test]
        public void ThenItShouldReturnValidResults()
        {
            _submissionEventsRepository.Setup(r => r.GetSubmissionEventsForUln(It.IsAny<long>(), It.IsAny<long>()))
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