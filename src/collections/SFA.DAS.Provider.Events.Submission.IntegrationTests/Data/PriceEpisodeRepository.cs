using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class PriceEpisodeRepository
    {
        public static void Create(PriceEpisodeEntity priceEpisode)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Reference.PriceEdpisodes VALUES (@PriceEpisodeIdentifier,@Ukprn,@LearnRefNumber,@PriceEpisodeAimSeqNumber," +
                                   "@EpisodeEffectiveTNPStartDate,@TNP1,@TNP2,@TNP3,@TNP4,@CommitmentId,@EmpId)",
                                   priceEpisode);
            }
        }
    }
}
