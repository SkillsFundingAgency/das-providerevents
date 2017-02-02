using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation.Rules;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryRequestValidator
{
    public class WhenValidating
    {
        private const string PageNumberMessage = "Page number violation";
        private const string BothFilterMessage = "You can specify SinceEventId or SinceTime or neither. You cannot specify both";

        private GetSubmissionEventsQueryRequest _request;
        private Mock<PageNumberMustBeAtLeastOneRule> _pageNumberMustBeAtLeastOneRule;
        private Application.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _request = new GetSubmissionEventsQueryRequest
            {
                PageNumber = 0,
                SinceEventId = 1,
                SinceTime = DateTime.Now
            };

            _pageNumberMustBeAtLeastOneRule = new Mock<PageNumberMustBeAtLeastOneRule>();
            _pageNumberMustBeAtLeastOneRule.Setup(r => r.Validate(It.IsAny<int>()))
                .ReturnsAsync(PageNumberMessage);

            _validator = new Application.Submissions.GetSubmissionEventsQuery.GetSubmissionEventsQueryRequestValidator(_pageNumberMustBeAtLeastOneRule.Object);
        }

        [Test]
        public async Task AndNoRulesFailThenItShouldReturnAValidResponse()
        {
            // Arrange
            _pageNumberMustBeAtLeastOneRule.Setup(r => r.Validate(It.IsAny<int>()))
                .ReturnsAsync(null);
            _request = new GetSubmissionEventsQueryRequest
            {
                PageNumber = 1
            };

            // Act
            var actual = await _validator.Validate(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task AndThePageNumberRuleFailsThenItShouldReturnAnInvalidResultWithErrorMessage()
        {
            // Act
            var actual = await _validator.Validate(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.ValidationMessages);
            Assert.Contains(PageNumberMessage, actual.ValidationMessages);
        }

        [Test]
        public async Task AndBothFiltersAreSpecifiedThenItShouldReturnAnInvalidResultWithErrorMessage()
        {
            // Act
            var actual = await _validator.Validate(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.ValidationMessages);
            Assert.Contains(BothFilterMessage, actual.ValidationMessages);
        }
    }
}
