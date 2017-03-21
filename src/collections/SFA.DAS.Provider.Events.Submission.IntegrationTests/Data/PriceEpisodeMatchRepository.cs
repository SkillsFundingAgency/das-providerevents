using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class PriceEpisodeMatchRepository
    {
        public static void Create(PriceEpisodeMatchEntity match)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO DataLock.PriceEpisodeMatch (Ukprn, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, CommitmentId, IsSuccess) " +
                                   "VALUES " +
                                   "(@Ukprn, @PriceEpisodeIdentifier, @LearnRefNumber, @AimSeqNumber, @CommitmentId, @IsSuccess)", match);
            }
        }

        public static void Clean()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("DELETE FROM DataLock.PriceEpisodeMatch");
            }
        }
    }
}