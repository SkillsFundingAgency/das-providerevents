using System.Threading.Tasks;

namespace SFA.DAS.Payments.Events.Application.Validation
{
    public interface IValidator<T>
    {
        Task<ValidationResult> Validate(T item);
    }
}