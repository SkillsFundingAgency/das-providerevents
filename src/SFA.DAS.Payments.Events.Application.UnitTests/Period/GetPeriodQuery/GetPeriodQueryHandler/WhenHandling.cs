using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Domain.Data;

namespace SFA.DAS.Payments.Events.Application.UnitTests.Period.GetPeriodQuery.GetPeriodQueryHandler
{
    public class WhenHandling
    {
        private GetPeriodQueryRequest _request;
        private Mock<IValidator<GetPeriodQueryRequest>> _requestValidator;
        private Mock<IPeriodRepository> _periodRepository;
        private Application.Period.GetPeriodQuery.GetPeriodQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _request = new GetPeriodQueryRequest
            {
                PeriodId = "1617-R02"
            };

            _requestValidator = new Mock<IValidator<GetPeriodQueryRequest>>();
            _requestValidator.Setup(v => v.Validate(It.IsAny<GetPeriodQueryRequest>()))
                .Returns(Task.FromResult(new ValidationResult()));

            _periodRepository = new Mock<IPeriodRepository>();
            _periodRepository.Setup(r => r.GetPeriod("1617", "R02"))
                .Returns(Task.FromResult(new Domain.Data.Entities.PeriodEntity
                {
                    Id = "1617-R02",
                    CalendarMonth = 9,
                    CalendarYear = 2016
                }));

            _handler = new Application.Period.GetPeriodQuery.GetPeriodQueryHandler(_requestValidator.Object, _periodRepository.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAValidResponseWithThePeriod()
        {
            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual("1617-R02", actual.Result.Id);
            Assert.AreEqual(9, actual.Result.CalendarMonth);
            Assert.AreEqual(2016, actual.Result.CalendarYear);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResponseWithAValidationExceptionIfMessageIsInvalid()
        {
            // Arrange
            _requestValidator.Setup(v => v.Validate(It.IsAny<GetPeriodQueryRequest>()))
                .Returns(Task.FromResult(new ValidationResult
                {
                    ValidationMessages = new[] { "Message not valid", "Message really not valid" }
                }));

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.IsInstanceOf<ValidationException>(actual.Exception);

            var validationException = (ValidationException)actual.Exception;
            Assert.AreEqual("Message not valid\nMessage really not valid", validationException.Message);
            Assert.IsNotNull(validationException.ValidationMessages);
            Assert.AreEqual(2, validationException.ValidationMessages.Length);
            Assert.AreEqual("Message not valid", validationException.ValidationMessages[0]);
            Assert.AreEqual("Message really not valid", validationException.ValidationMessages[1]);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResponseWithAnExceptionIfRepositoryThrowsException()
        {
            // Arrange
            var ex = new Exception("Test");
            _periodRepository.Setup(r => r.GetPeriod("1617", "R02"))
                .Throws(ex);

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreSame(ex, actual.Exception);
        }
    }
}
