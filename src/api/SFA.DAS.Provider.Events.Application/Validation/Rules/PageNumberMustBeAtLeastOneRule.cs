using System;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Validation.Rules
{
    public class PageNumberMustBeAtLeastOneRule : IValidationRule<int>
    {
        public virtual Task<string> Validate(int pageNumber)
        {
            if (pageNumber < 1)
            {
                return Task.FromResult("Page number must be 1 or more");
            }

            return Task.FromResult<string>(null);
        }
    }
}
