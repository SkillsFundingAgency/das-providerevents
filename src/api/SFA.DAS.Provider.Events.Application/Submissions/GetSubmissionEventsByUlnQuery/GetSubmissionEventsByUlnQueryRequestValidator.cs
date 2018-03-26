using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery
{
    public class GetSubmissionEventsByUlnQueryRequestValidator : IValidator<GetSubmissionEventsByUlnQueryRequest>
    {
        public async Task<ValidationResult> Validate(GetSubmissionEventsByUlnQueryRequest item)
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
