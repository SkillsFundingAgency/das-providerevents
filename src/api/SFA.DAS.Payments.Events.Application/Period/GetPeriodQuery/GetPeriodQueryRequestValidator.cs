using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Application.Validation.Rules;

namespace SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryRequestValidator : IValidator<GetPeriodQueryRequest>
    {
        private readonly PeriodIdFormatValidationRule _periodIdFormatValidationRule;

        public GetPeriodQueryRequestValidator(PeriodIdFormatValidationRule periodIdFormatValidationRule)
        {
            _periodIdFormatValidationRule = periodIdFormatValidationRule;
        }

        public async Task<ValidationResult> Validate(GetPeriodQueryRequest item)
        {
            var validationErrors = new[]
            {
                await _periodIdFormatValidationRule.Validate(item.PeriodId)
            };

            return new ValidationResult
            {
                ValidationMessages = validationErrors.Where(e => !string.IsNullOrEmpty(e)).ToArray()
            };
        }
    }
}
