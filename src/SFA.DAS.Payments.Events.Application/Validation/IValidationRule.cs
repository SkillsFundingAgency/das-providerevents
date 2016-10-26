using System.Threading.Tasks;

namespace SFA.DAS.Payments.Events.Application.Validation
{
    public interface IValidationRule<T>
    {
        Task<string> Validate(T item);
    }
}