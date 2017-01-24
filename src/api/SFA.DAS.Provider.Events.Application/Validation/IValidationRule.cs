using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Validation
{
    public interface IValidationRule<T>
    {
        Task<string> Validate(T item);
    }
}