using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Validation.Rules.PeriodIdFormatValidationRule
{
    public class WhenValidating
    {
        private Application.Validation.Rules.PeriodIdFormatValidationRule _rule;

        [SetUp]
        public void Arrange()
        {
            _rule = new Application.Validation.Rules.PeriodIdFormatValidationRule();
        }

        [TestCase("1617-R01")]
        [TestCase("2021-R12")]
        public async Task WithAValidPeriodIdThenItShouldReturnNull(string periodId)
        {
            // Act
            var actual = await _rule.Validate(periodId);

            // Assert
            Assert.IsNull(actual);
        }

        [TestCase("AAAA")]
        [TestCase("AAAA-R02")]
        [TestCase("1617-XXX")]
        [TestCase("1617-02")]
        [TestCase("1617-R2")]
        public async Task WithAnInvalidPeriodIdThenItShouldReturnInvalidMessage(string periodId)
        {
            // Act
            var actual = await _rule.Validate(periodId);

            // Assert
            Assert.AreEqual("Period Id is not in a valid format. Excepted format is [AcademicYear]-[Period]; e.g. 1617-R01", actual);
        }
    }
}
