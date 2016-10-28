using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Payments.Events.Application.Validation.Rules;

namespace SFA.DAS.Payments.Events.Application.UnitTests.Period.GetPeriodQuery.GetPeriodQueryRequestValidator
{
    public class WhenValidating
    {
        private GetPeriodQueryRequest _request;
        private Mock<PeriodIdFormatValidationRule> _periodIdFormatValidationRule;
        private Application.Period.GetPeriodQuery.GetPeriodQueryRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _request = new GetPeriodQueryRequest();

            _periodIdFormatValidationRule = new Mock<PeriodIdFormatValidationRule>();

            _validator = new Application.Period.GetPeriodQuery.GetPeriodQueryRequestValidator(_periodIdFormatValidationRule.Object);
        }

        [Test]
        public async Task AndNoRulesFailThenItShouldReturnAValidResponse()
        {
            // Act
            var actual = await _validator.Validate(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task AnPeriodIdIsInvalidThenItShouldReturnAnInvalidResponseIncludingPeriodIdError()
        {
            // Arrange
            _periodIdFormatValidationRule.Setup(r => r.Validate(It.IsAny<string>()))
                .Returns(Task.FromResult("Period Id Invalid"));

            // Act
            var actual = await _validator.Validate(_request);

            // Assert
            Assert.IsFalse(actual.IsValid);
            Assert.Contains("Period Id Invalid", actual.ValidationMessages);
        }
    }
}
