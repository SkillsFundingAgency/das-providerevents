using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventByStandardQueryRequestValidator : IValidator<GetLatestLearnerEventForStandardsQueryRequest>
    {
        public async Task<ValidationResult> Validate(GetLatestLearnerEventForStandardsQueryRequest item)
        {
            var validationErrors = new List<string>();

            if (item.Uln < 1000000000 || item.Uln > 9999999999)
            {
                validationErrors.Add("Uln must be between 1111111111 and 9999999999");
            }
            else if (!IsValidCheckDigit(item.Uln))
            {
                validationErrors.Add("Uln is not valid. Check digit incorrect");
            }

            return new ValidationResult
            {
                ValidationMessages = validationErrors.Where(e => !string.IsNullOrEmpty(e)).ToArray()
            };
        }

        private bool IsValidCheckDigit(long uln)
        {
            var seed = uln.ToString().Substring(0, 9);
            var suppliedCheckDigit = int.Parse(uln.ToString().Substring(9, 1));

            var multiplyBy = 10;
            var total = 0;

            for (int i = 0; i < 9; i++)
            {
                total += int.Parse(seed[i].ToString()) * multiplyBy;
                multiplyBy--;
            }

            var calculatedCheckdigit = 10 - (total % 11);
            return calculatedCheckdigit == suppliedCheckDigit;
        }
    }
}
