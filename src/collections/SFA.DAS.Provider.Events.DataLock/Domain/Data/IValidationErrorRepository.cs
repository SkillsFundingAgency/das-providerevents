using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IValidationErrorRepository
    {
        ValidationErrorEntity[] GetPriceEpisodeValidationErrors(long ukprn, string priceEpisodeIdentifier, string learnRefNumber);
    }
}