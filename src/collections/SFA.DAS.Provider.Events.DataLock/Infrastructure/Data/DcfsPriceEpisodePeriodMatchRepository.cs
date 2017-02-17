using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsPriceEpisodePeriodMatchRepository : DcfsRepository, IPriceEpisodePeriodMatchRepository
    {
        private const string Source = "Reference.PriceEpisodePeriodMatch";
        private const string Columns = "Ukprn," +
                                       "PriceEpisodeIdentifier," +
                                       "LearnRefnumber," +
                                       "AimSeqNumber," +
                                       "CommitmentId," +
                                       "VersionId," +
                                       "Period," +
                                       "Payable," +
                                       "TransactionType";
        private const string SelectPriceEpisodePeriodMatches = "SELECT " + Columns + " FROM " + Source +
            " WHERE Ukprn = @ukprn AND PriceEpisodeIdentifier = @priceEpisodeIdentifier AND LearnRefnumber = @learnRefnumber";

        public DcfsPriceEpisodePeriodMatchRepository(string connectionString)
            : base(connectionString)
        {
        }

        public PriceEpisodePeriodMatchEntity[] GetPriceEpisodePeriodMatches(long ukprn, string priceEpisodeIdentifier, string learnRefNumber)
        {
            return Query<PriceEpisodePeriodMatchEntity>(SelectPriceEpisodePeriodMatches, new { ukprn, priceEpisodeIdentifier, learnRefNumber });
        }
    }
}