using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsIlrPriceEpisodeRepository : DcfsRepository, IIlrPriceEpisodeRepository
    {
        private const string Source = "DataLockEvents.vw_IlrPriceEpisodes";
        private const string Columns = "IlrFilename," +
                                       "SubmittedTime," +
                                       "Ukprn," +
                                       "Uln," +
                                       "PriceEpisodeIdentifier," +
                                       "LearnRefNumber," +
                                       "AimSeqNumber," +
                                       "IlrStartDate," +
                                       "IlrStandardCode," +
                                       "IlrProgrammeType," +
                                       "IlrFrameworkCode," +
                                       "IlrPathwayCode," +
                                       "IlrTrainingPrice," +
                                       "IlrEndpointAssessorPrice," +
                                       "IlrPriceEffectiveDate";
        private const string SelectPriceEpisodeIlrData = "SELECT " + Columns + " FROM " + Source +
            " WHERE Ukprn = @ukprn AND PriceEpisodeIdentifier = @priceEpisodeIdentifier AND LearnRefnumber = @learnRefnumber";

        public DcfsIlrPriceEpisodeRepository(string connectionString)
            : base(connectionString)
        {
        }

        public IlrPriceEpisodeEntity GetPriceEpisodeIlrData(long ukprn, string priceEpisodeIdentifier, string learnRefNumber)
        {
            return QuerySingle<IlrPriceEpisodeEntity>(SelectPriceEpisodeIlrData, new {ukprn, priceEpisodeIdentifier, learnRefNumber});
        }
    }
}