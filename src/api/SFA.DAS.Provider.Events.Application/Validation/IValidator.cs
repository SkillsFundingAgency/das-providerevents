using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Validation
{
    public interface IValidator<T>
    {
        Task<ValidationResult> Validate(T item);
    }
}