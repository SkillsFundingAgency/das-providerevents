using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Submissions.GetSubmissionEventsByUlnQuery.GetSubmissionEventsByUlnQueryRequestValidator
{
    public class WhenValidating
    {
        private Application.Submissions.GetSubmissionEventsByUlnQuery.GetSubmissionEventsByUlnQueryRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Application.Submissions.GetSubmissionEventsByUlnQuery.
                GetSubmissionEventsByUlnQueryRequestValidator();
        }

        [TestCase(999999999)]
        [TestCase(0)]
        [TestCase(10000000000)]
        public void ThenAnInvalidValueForUlnShouldReturnInvalid(long ulnValue)
        {
            var result = _validator.Validate(new GetSubmissionEventsByUlnQueryRequest(){SinceEventId = 0, Uln = ulnValue });

            result.Result.IsValid.Should().BeFalse();
        }

        [TestCase(1000000000)]
        [TestCase(5555555555)]
        [TestCase(9999999999)]
        public void ThenAnValidValueForUlnShouldReturnValid(long ulnValue)
        {
            var result = _validator.Validate(new GetSubmissionEventsByUlnQueryRequest() { SinceEventId = 0, Uln = ulnValue });

            result.Result.IsValid.Should().BeTrue();
        }
    }
}