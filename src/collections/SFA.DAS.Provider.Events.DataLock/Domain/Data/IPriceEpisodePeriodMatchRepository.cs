using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IPriceEpisodePeriodMatchRepository
    {
        PriceEpisodePeriodMatchEntity[] GetPriceEpisodePeriodMatches(long ukprn, string priceEpisodeIdentifier, string learnRefNumber);
    }
}