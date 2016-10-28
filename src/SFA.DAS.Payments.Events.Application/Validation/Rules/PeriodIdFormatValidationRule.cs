using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Events.Application.Validation.Rules
{
    public class PeriodIdFormatValidationRule : IValidationRule<string>
    {
        private const string ValidPeriodIdFormat = @"^[0-9]{4}\-R[0-9]{2}$";

        public virtual Task<string> Validate(string item)
        {
            if (!Regex.IsMatch(item, ValidPeriodIdFormat))
            {
                return Task.FromResult("Period Id is not in a valid format. Excepted format is [AcademicYear]-[Period]; e.g. 1617-R01");
            }
            return Task.FromResult<string>(null);
        }
    }
}
