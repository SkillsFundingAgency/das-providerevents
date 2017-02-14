using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface IIlrPriceEpisodeRepository
    {
        IlrPriceEpisodeEntity GetPriceEpisodeIlrData(long ukprn, string priceEpisodeIdentifier, string learnRefNumber);
    }
}