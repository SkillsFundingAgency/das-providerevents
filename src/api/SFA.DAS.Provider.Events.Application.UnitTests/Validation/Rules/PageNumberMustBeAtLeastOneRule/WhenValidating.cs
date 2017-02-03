using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Validation.Rules.PageNumberMustBeAtLeastOneRule
{
    public class WhenValidating
    {
        private Application.Validation.Rules.PageNumberMustBeAtLeastOneRule _rule;

        [SetUp]
        public void Arrange()
        {
            _rule = new Application.Validation.Rules.PageNumberMustBeAtLeastOneRule();
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(928381324)]
        public async Task WithAValidPageNumberItShouldReturnNull(int pageNumber)
        {
            // Act
            var actual = await _rule.Validate(pageNumber);

            // Assert
            Assert.IsNull(actual);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-193837)]
        public async Task WithAnInvalidPageNumberItShouldReturnErrorMessage(int pageNumber)
        {
            // Act
            var actual = await _rule.Validate(pageNumber);

            // Assert
            Assert.AreEqual("Page number must be 1 or more", actual);
        }
    }
}
