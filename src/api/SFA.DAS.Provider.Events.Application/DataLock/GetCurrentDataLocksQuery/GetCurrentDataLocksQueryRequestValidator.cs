using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Application.Validation.Rules;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery
{
    public class GetCurrentDataLocksQueryRequestValidator : IValidator<GetCurrentDataLocksQueryRequest>
    {
        private readonly PageNumberMustBeAtLeastOneRule _pageNumberMustBeAtLeastOneRule;

        public GetCurrentDataLocksQueryRequestValidator(PageNumberMustBeAtLeastOneRule pageNumberMustBeAtLeastOneRule)
        {
            _pageNumberMustBeAtLeastOneRule = pageNumberMustBeAtLeastOneRule;
        }

        public async Task<ValidationResult> Validate(GetCurrentDataLocksQueryRequest item)
        {
            var validationErrors = new[]
            {
                await _pageNumberMustBeAtLeastOneRule.Validate(item.PageNumber),
            };

            return new ValidationResult
            {
                ValidationMessages = validationErrors.Where(e => !string.IsNullOrEmpty(e)).ToArray()
            };
        }
    }
}