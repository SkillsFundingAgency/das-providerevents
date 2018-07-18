using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetLatestLearnerEventByStandardQuery.GetLatestLearnerEventByStandardQueryRequestValidator
{
    public class WhenValidating
    {
        private Application.Submissions.GetLatestLearnerEventByStandardQuery.GetLatestLearnerEventByStandardQueryRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Application.Submissions.GetLatestLearnerEventByStandardQuery.GetLatestLearnerEventByStandardQueryRequestValidator();
        }

        [TestCase(999999999)]
        [TestCase(0)]
        [TestCase(10000000000)]
        [TestCase(1002216014)]
        [TestCase(9999999999)]
        [TestCase(5555555555)]
        public void ThenAnInvalidValueForUlnShouldReturnInvalid(long ulnValue)
        {
            var result = _validator.Validate(new GetLatestLearnerEventByStandardQueryRequest(){SinceEventId = 0, Uln = ulnValue });

            result.Result.IsValid.Should().BeFalse();
        }

        [TestCase(1000000000)]
        [TestCase(5555555554)]
        [TestCase(9999999998)]
        [TestCase(1002116014)]
        public void ThenAnValidValueForUlnShouldReturnValid(long ulnValue)
        {
            var result = _validator.Validate(new GetLatestLearnerEventByStandardQueryRequest() { SinceEventId = 0, Uln = ulnValue });

            result.Result.IsValid.Should().BeTrue();
        }
    }
}