using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventByStandardQueryRequestValidator : IValidator<GetLatestLearnerEventByStandardQueryRequest>
    {
        public async Task<ValidationResult> Validate(GetLatestLearnerEventByStandardQueryRequest item)
        {
            var validationErrors = new[]
            {
                item.Uln < 1000000000 || item.Uln > 9999999999 ? "Uln must be between 1111111111 and 9999999999": null
            };

            return new ValidationResult
            {
                ValidationMessages = validationErrors.Where(e => !string.IsNullOrEmpty(e)).ToArray()
            };
        }
    }
}
