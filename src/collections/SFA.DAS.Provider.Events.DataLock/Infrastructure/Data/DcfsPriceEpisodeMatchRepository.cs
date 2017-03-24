using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsPriceEpisodeMatchRepository : DcfsRepository, IPriceEpisodeMatchRepository
    {
        private const string Source = "DataLock.PriceEpisodeMatch";
        private const string Columns = "Ukprn," +
                                       "PriceEpisodeIdentifier," +
                                       "LearnRefNumber," +
                                       "AimSeqNumber," +
                                       "CommitmentId," +
                                       "IsSuccess";
        private const string SelectProviderPriceEpisodeMatches = "SELECT " + Columns + " FROM " + Source + " WHERE Ukprn = @ukprn";

        public DcfsPriceEpisodeMatchRepository(string connectionString)
            : base(connectionString)
        {
        }

        public PriceEpisodeMatchEntity[] GetProviderPriceEpisodeMatches(long ukprn)
        {
            return Query<PriceEpisodeMatchEntity>(SelectProviderPriceEpisodeMatches, new { ukprn });
        }
    }
}