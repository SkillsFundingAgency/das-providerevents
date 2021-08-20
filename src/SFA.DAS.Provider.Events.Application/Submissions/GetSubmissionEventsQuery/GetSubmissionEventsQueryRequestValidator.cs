using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Application.Validation.Rules;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery
{
    public class GetSubmissionEventsQueryRequestValidator : IValidator<GetSubmissionEventsQueryRequest>
    {
        private readonly PageNumberMustBeAtLeastOneRule _pageNumberMustBeAtLeastOneRule;

        public GetSubmissionEventsQueryRequestValidator(PageNumberMustBeAtLeastOneRule pageNumberMustBeAtLeastOneRule)
        {
            _pageNumberMustBeAtLeastOneRule = pageNumberMustBeAtLeastOneRule;
        }

        public async Task<ValidationResult> Validate(GetSubmissionEventsQueryRequest item)
        {
            var validationErrors = new[]
            {
                await _pageNumberMustBeAtLeastOneRule.Validate(item.PageNumber),
                ValidateBothFiltersAreNotSpecified(item)
            };

            return new ValidationResult
            {
                ValidationMessages = validationErrors.Where(e => !string.IsNullOrEmpty(e)).ToArray()
            };
        }


        private string ValidateBothFiltersAreNotSpecified(GetSubmissionEventsQueryRequest request)
        {
            if (request.SinceEventId > 0 && request.SinceTime.HasValue)
            {
                return "You can specify SinceEventId or SinceTime or neither. You cannot specify both";
            }
            return null;
        }
    }
}
