using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IProviderRepository
    {
        ProviderEntity[] GetAllProviders();
    }
}