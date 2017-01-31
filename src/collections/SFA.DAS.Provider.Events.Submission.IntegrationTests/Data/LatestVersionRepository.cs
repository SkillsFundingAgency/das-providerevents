using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public static class LatestVersionRepository
    {
        public static LatestVersionEntity GetLastestVersionForProvider(long ukprn)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                return connection.Query<LatestVersionEntity>("SELECT * FROM Submissions.LatestVersion WHERE UKPRN=@Ukprn", new { ukprn }).SingleOrDefault();
            }
        }

        public static void Create(LatestVersionEntity latestVersion)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Submissions.LatestVersion VALUES (@IlrFileName,@FileDateTime,@SubmittedDateTime," +
                                   "@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@PriceEpisodeIdentifier,@StandardCode," +
                                   "@ProgrammeType,@FrameworkCode,@PathwayCode,@ActualStartDate,@PlannedEndDate,@ActualEndDate," +
                                   "@OnProgrammeTotalPrice,@CompletionTotalPrice,@NINumber)", latestVersion);
            }
        }
    }
}
