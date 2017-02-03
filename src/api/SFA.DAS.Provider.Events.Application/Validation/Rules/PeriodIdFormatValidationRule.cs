using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Validation.Rules
{
    public class PeriodIdFormatValidationRule : IValidationRule<string>
    {
        private const string ValidPeriodIdFormat = @"^[0-9]{4}\-R[0-9]{2}$";

        public virtual Task<string> Validate(string pageNumber)
        {
            if (!Regex.IsMatch(pageNumber, ValidPeriodIdFormat))
            {
                return Task.FromResult("Period Id is not in a valid format. Excepted format is [AcademicYear]-[Period]; e.g. 1617-R01");
            }
            return Task.FromResult<string>(null);
        }
    }
}
