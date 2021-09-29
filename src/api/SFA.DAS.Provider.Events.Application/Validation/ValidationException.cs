using System;
using System.Linq;

namespace SFA.DAS.Provider.Events.Application.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string[] validationMessages)
            : base(AggregateValidationMessages(validationMessages))
        {
            ValidationMessages = validationMessages;
        }

        public string[] ValidationMessages { get; }

        private static string AggregateValidationMessages(string[] validationMessages)
        {
            return validationMessages.Aggregate((x, y) => $"{x}\n{y}");
        }
    }
}
